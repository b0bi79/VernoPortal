using Abp.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public class ReportColumnRepository : ReportsRepositoryBase<RepParameter, int>
    {
        public ReportColumnRepository(IDbContextProvider<ReportsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}