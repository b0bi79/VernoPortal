using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Verno.Identity.Users.Dto;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Transactions;
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Verno.Identity.Users
{
    [AbpAuthorize("Administration.UserManagement")]
    public class UserAppService : IdentityAppServiceBase, IUserAppService
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IEmailSender _emailSender;
        private readonly string _rootUrl;
        private readonly string _appName;

        public UserAppService(IPermissionManager permissionManager, IEmailSender emailSender, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
        {
            _permissionManager = permissionManager;
            _emailSender = emailSender;
            var request = contextAccessor.HttpContext.Request;
            _rootUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            _appName = env.ApplicationName;
        }

        [AbpAuthorize("Administration.UserManagement.EditPermissions")]
        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await UserManager.FindByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

        //Example for primitive method parameters.
        [AbpAuthorize("Administration.UserManagement.EditRoles")]
        public async Task UpdateRoles(int userId, string[] roleNames)
        {
            var user = await UserManager.FindByIdAsync(userId);
            CheckErrors(await UserManager.UpdateRolesAsync(user, roleNames));
        }

        //Example for primitive method parameters.
        [AbpAuthorize("Administration.UserManagement.EditRoles")]
        public async Task<ListResultOutput<string>> GetRoles(int userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var roles = await UserManager.GetRolesAsync(user);
            return new ListResultOutput<string>(roles.ToList());
        }

        public ListResultOutput<UserDto> GetAll()
        {
            return new ListResultOutput<UserDto>(
                UserManager.Users.GetAllActive()
                    .Include(x => x.Claims)
                    .Include(x => x.OrgUnit)
                    .OrderBy(t => t.Name)
                    .ToList()
                    .MapTo<List<UserDto>>()
                );
        }

        [AbpAuthorize("Administration.UserManagement.CreateUser")]
        public async Task<UserDto> Create(UserDto input)
        {
            //Create user
            var user = new User(input.UserName)
            {
                Name = input.Name,
                Email = input.Email,
                OrgUnitId = input.OrgUnitId,
            };

            CheckErrors(await UserManager.CreateAsync(user));
            if (input.Position != null) CheckErrors(await UserManager.AddClaimAsync(user, new Claim(UserClaimTypes.Position, input.Position)));
            if (input.BossId != null) CheckErrors(await UserManager.AddClaimAsync(user, new Claim(UserClaimTypes.BossId, input.BossId)));
            if (input.ShopNum.HasValue) CheckErrors(await UserManager.AddClaimAsync(user, new Claim(UserClaimTypes.ShopNum, input.ShopNum.Value.ToString())));
            //CheckErrors(await UserManager.AddPasswordAsync(user, User.DefaultPassword));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's id.
            input.Id = user.Id;

            try
            {
                var callbackUrl = _rootUrl + $"/Account/JoinIn?userName={UrlEncoder.Default.Encode(user.UserName)}";
                await _emailSender.SendAsync(user.Email, $"Доступ в {_appName}",
                    $"Поздравляем, вы зарегистрированы в системе {_appName}, теперь вы можете войти на сайт нажав на ссылку: <a href='{callbackUrl}'>{_appName}</a>.");
            }
            catch (SmtpException ex)
            {
                Logger.Error("Can't send email.", ex);
            }
            return input;
        }

        [AbpAuthorize("Administration.UserManagement.UpdateUser")]
        public async Task<UserDto> Update(UserDto input)
        {
            var user = await UserManager.Users.Include(x => x.Claims).FirstOrDefaultAsync(x => x.Id == input.Id);
            input.MapTo(user);
            user.OrgUnit = null;

            await UpdateClaim(user, UserClaimTypes.BossId, input.BossId);
            await UpdateClaim(user, UserClaimTypes.Position, input.Position);
            await UpdateClaim(user, UserClaimTypes.ShopNum, input.ShopNum?.ToString());

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
                return input;
            else
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        [AbpAuthorize("Administration.UserManagement.DeleteUser")]
        public async Task<UserDto> Delete(UserDto input)
        {
            var user = await UserManager.FindByIdAsync(input.Id);
            user.IsActive = false;
            CheckErrors(await UserManager.UpdateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();
            return input;
        }

        [AbpAuthorize("Administration.UserManagement.ResetPassword")]
        public async Task PasswordReset(PasswordResetInput input)
        {
            var user = await UserManager.FindByIdAsync(input.UserId);

            CheckErrors(await UserManager.RemovePasswordAsync(user));
            if (!input.NewPassword.IsNullOrWhiteSpace())
            {
                CheckErrors(await UserManager.AddPasswordAsync(user, input.NewPassword));

                try
                {
                    var curUser = await GetCurrentUserAsync();
                    await _emailSender.SendAsync(user.Email, $"Изменён пароль к {_appName}",
                        $"Ваш пароль в системе {_appName} был изменён администратором ({curUser.Name}). " +
                        $"Что бы узнать текущий пароль обратитесь к администратору или по электронной почте " +
                        $"<a href='mailto:{curUser.Email}'>{curUser.Email}</a>.");
                }
                catch (SmtpException ex)
                {
                    Logger.Error("Can't send email.", ex);
                }
            }
            else
                try
                {
                    var callbackUrl = _rootUrl + $"/Account/JoinIn?userName={UrlEncoder.Default.Encode(user.UserName)}";
                    var curUser = await GetCurrentUserAsync();
                    await _emailSender.SendAsync(user.Email, $"Сброшен пароль к {_appName}",
                        $"Ваш пароль в системе {_appName} был сброшен администратором ({curUser.Name}). " +
                        $"Что бы задать новый пароль перейдите по ссылке: " +
                        $"<a href='{callbackUrl}'>Задать новый пароль</a>.");
                }
                catch (SmtpException ex)
                {
                    Logger.Error("Can't send email.", ex);
                }
        }

        private async Task UpdateClaim(User user, ClaimType claimType, string inputValue)
        {
            var claim = user.Claims.FirstOrDefault(x => x.ClaimType == claimType);
            if (claim != null)
            {
                if (string.IsNullOrEmpty(inputValue))
                    CheckErrors(await UserManager.RemoveClaimAsync(user, claim.ToClaim()));
                else
                    claim.ClaimValue = inputValue;
            }
            else
            {
                if (!string.IsNullOrEmpty(inputValue))
                    CheckErrors(await UserManager.AddClaimAsync(user, new Claim(claimType, inputValue)));
            }
        }
    }
}