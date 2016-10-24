using Abp.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public class RolePermissionsRepository: ReportsRepositoryBase<Report>
    {
        public RolePermissionsRepository(IDbContextProvider<ReportsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}