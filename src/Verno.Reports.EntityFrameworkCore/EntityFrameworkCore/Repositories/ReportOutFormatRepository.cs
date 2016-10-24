using Abp.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public class ReportOutFormatRepository : ReportsRepositoryBase<ReportOutFormat>
    {
        public ReportOutFormatRepository(IDbContextProvider<ReportsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}