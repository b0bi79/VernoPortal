using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Verno.Identity.Users
{
    public class SignInManager : SignInManager<User>
    {
        private readonly IRepository<UserLoginAttempt, int> _userLoginAttemptRepository;
        private readonly string _currApplication;

        public SignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IRepository<UserLoginAttempt, int> userLoginAttemptRepository, string application)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            _userLoginAttemptRepository = userLoginAttemptRepository;
            _currApplication = application;
        }

        public IAuditInfoProvider AuditInfoProvider { get; set; }

        /// <inheritdoc />
        public override async Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            if (result == SignInResult.Failed)
                await SaveLoginAttempt(user?.Id, $"Wrong password '{password}'.", user?.Name);
            else
                await SaveLoginAttempt(user?.Id, result.ToString(), user?.Name);
            return result;
        }

        /// <inheritdoc />
        public override async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
        {
            var result = await base.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
            var user = await GetTwoFactorAuthenticationUserAsync();
            await SaveLoginAttempt(user?.Id, result.ToString(), user?.Name);
            return result;
        }

        /// <inheritdoc />
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var byNameAsync = await this.UserManager.FindByNameAsync(userName);
            if ((object)byNameAsync == null)
            {
                await SaveLoginAttempt(null, "Login not found.", userName);
                return SignInResult.Failed;
            }
            return await this.PasswordSignInAsync(byNameAsync, password, isPersistent, lockoutOnFailure);
        }

        private async Task SaveLoginAttempt(int? userId, string loginResult, string userNameOrEmailAddress)
        {
            var loginAttempt = new UserLoginAttempt()
            {
                UserId = userId,
                UserNameOrEmailAddress = userNameOrEmailAddress,
                Result = loginResult,
                Application = _currApplication,
            };
            if (AuditInfoProvider != null)
            {
                var auditInfo = new AuditInfo();
                AuditInfoProvider.Fill(auditInfo);
                loginAttempt.BrowserInfo = auditInfo.BrowserInfo;
                loginAttempt.ClientIpAddress = auditInfo.ClientIpAddress;
                loginAttempt.ClientName = auditInfo.ClientName;
            }
            await _userLoginAttemptRepository.InsertAsync(loginAttempt);
        }
    }
}