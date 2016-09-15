using System.Threading.Tasks;
using Verno.Identity.Account.Dto;
using Verno.Identity.Users;

namespace Verno.Identity.Account
{
    public class AccountAppService : IdentityAppServiceBase, IAccountAppService
    {
        private readonly SignInManager _signInManager;

        public AccountAppService(SignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task ChangePassword(ChangePasswordInput model)
        {
            var user = await GetCurrentUserAsync();

            CheckErrors(await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword));
            await _signInManager.SignInAsync(user, isPersistent: false);
            Logger.Info("User changed their password successfully.");
        }

        public async Task SetPassword(SetPasswordInput model)
        {
            var user = await GetCurrentUserAsync();

            CheckErrors(await UserManager.AddPasswordAsync(user, model.NewPassword));
            await _signInManager.SignInAsync(user, isPersistent: false);
            Logger.Info("User changed their password successfully.");
        }

        public async Task<bool> HasPassword()
        {
            var user = await GetCurrentUserAsync();
            return await UserManager.HasPasswordAsync(user);
        }
    }
}