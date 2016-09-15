using System.Threading.Tasks;
using Verno.Identity.Account.Dto;

namespace Verno.Identity.Account
{
    public interface IAccountAppService
    {
        Task ChangePassword(ChangePasswordInput model);
        Task SetPassword(SetPasswordInput model);
        Task<bool> HasPassword();
    }
}