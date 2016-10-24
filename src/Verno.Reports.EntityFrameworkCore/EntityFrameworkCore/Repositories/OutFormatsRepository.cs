using Abp.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore.Repositories
{
    public class OutFormatRepository : ReportsRepositoryBase<OutFormat, string>
    {
        public OutFormatRepository(IDbContextProvider<ReportsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}