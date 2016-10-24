using Abp.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public class RepParameterRepository : ReportsRepositoryBase<RepParameter, int>
    {
        public RepParameterRepository(IDbContextProvider<ReportsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}