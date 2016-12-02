using Abp.AspNetCore;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Verno.Identity.Data;
using Verno.Identity.Roles;

namespace Verno.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static void ConfigureIdentity(this AbpServiceOptions options, string applicationName)
        {
            IdentityModule.ApplicationName = applicationName;
        }
    }

    public class RolePermissionSettingRepository : EfCoreRepositoryBase<IdentityDbContext, RolePermissionSetting, int>
    {
        public RolePermissionSettingRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}