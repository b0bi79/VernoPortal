using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Verno.Identity.Sessions.Dto;

namespace Verno.Identity.Sessions
{
    [AbpAuthorize]
    public class SessionAppService : IdentityAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var user = await GetCurrentUserAsync();
            var output = new GetCurrentLoginInformationsOutput
            {
                User = user.MapTo<UserLoginInfoDto>()
            };

            output.User.Roles = (await UserManager.GetRolesAsync(user)).ToArray();
            return output;
        }
    }
}