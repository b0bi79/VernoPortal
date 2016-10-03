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
using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Verno.Configuration;

namespace Verno.Identity.Users
{
    [AbpAuthorize("Administration.UserManagement")]
    public class UserAppService : IdentityAppServiceBase, IUserAppService
    {
        private const string ServiceUrl = "api/services/identity/users";
        private readonly IPermissionManager _permissionManager;
        private readonly IEmailSender _emailSender;
        private readonly string _rootUrl;
        private readonly AppSettings _appSettings;

        public UserAppService(IPermissionManager permissionManager, IEmailSender emailSender, IHttpContextAccessor contextAccessor, IOptions<AppSettings> appSettings)
        {
            _permissionManager = permissionManager;
            _emailSender = emailSender;
            var request = contextAccessor.HttpContext.Request;
            _rootUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            _appSettings = appSettings.Value;
        }

        [HttpDelete]
        [Route(ServiceUrl + "/{userId}/permissions/{permissionName}")]
        [AbpAuthorize("Administration.UserManagement.EditPermissions")]
        public async Task ProhibitPermission(int userId, string permissionName)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var permission = _permissionManager.GetPermission(permissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

        //Example for primitive method parameters.
        [HttpPut]
        [Route(ServiceUrl+"/{userId}/roles")]
        [AbpAuthorize("Administration.UserManagement.EditRoles")]
        public async Task UpdateRoles(int userId, string[] roleNames)
        {
            var user = await UserManager.FindByIdAsync(userId);
            CheckErrors(await UserManager.UpdateRolesAsync(user, roleNames));
        }

        //Example for primitive method parameters.
        [HttpGet]
        [Route(ServiceUrl + "/{userId}/roles")]
        [AbpAuthorize("Administration.UserManagement.EditRoles")]
        public async Task<ListResultOutput<string>> GetRoles(int userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var roles = await UserManager.GetRolesAsync(user);
            return new ListResultOutput<string>(roles.ToList());
        }

        [HttpGet]
        [Route(ServiceUrl)]
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

        [HttpPost]
        [Route(ServiceUrl)]
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
                await _emailSender.SendAsync(user.Email, $"������ � {_appSettings.SiteTitle}",
                    $"�����������, �� ���������������� � ������� {_appSettings.SiteTitle}, ������ �� ������ ����� �� ���� ����� �� ������: <a href='{callbackUrl}'>{_appSettings.SiteTitle}</a>.");
            }
            catch (SmtpException ex)
            {
                Logger.Error("Can't send email.", ex);
            }
            return input;
        }

        [HttpPut]
        [Route(ServiceUrl)]
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

        [HttpDelete]
        [Route(ServiceUrl)]
        [AbpAuthorize("Administration.UserManagement.DeleteUser")]
        public async Task<UserDto> Delete(int id)
        {
            var user = await UserManager.FindByIdAsync(id);
            user.IsActive = false;
            CheckErrors(await UserManager.UpdateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();
            return user.MapTo<UserDto>();
        }

        [HttpPost]
        [Route(ServiceUrl + "/{userId:int:min(1)}/PasswordReset")]
        [AbpAuthorize("Administration.UserManagement.ResetPassword")]
        public async Task PasswordReset(int userId, PasswordResetInput input)
        {
            var user = await UserManager.FindByIdAsync(userId);

            CheckErrors(await UserManager.RemovePasswordAsync(user));
            if (!input.NewPassword.IsNullOrWhiteSpace())
            {
                CheckErrors(await UserManager.AddPasswordAsync(user, input.NewPassword));

                try
                {
                    var curUser = await GetCurrentUserAsync();
                    await _emailSender.SendAsync(user.Email, $"������ ������ � {_appSettings.SiteTitle}",
                        $"��� ������ � ������� {_appSettings.SiteTitle} ��� ������ ��������������� ({curUser.Name}). " +
                        $"��� �� ������ ������� ������ ���������� � �������������� ��� �� ����������� ����� " +
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
                    await _emailSender.SendAsync(user.Email, $"������� ������ � {_appSettings.SiteTitle}",
                        $"��� ������ � ������� {_appSettings.SiteTitle} ��� ������� ��������������� ({curUser.Name}). " +
                        $"��� �� ������ ����� ������ ��������� �� ������: " +
                        $"<a href='{callbackUrl}'>������ ����� ������</a>.");
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