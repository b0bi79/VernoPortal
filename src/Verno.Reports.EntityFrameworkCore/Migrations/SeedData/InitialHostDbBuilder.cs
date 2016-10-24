using Verno.Reports.EntityFrameworkCore;

namespace Verno.Reports.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly ReportsDbContext _context;

        public InitialHostDbBuilder(ReportsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultConnectionsCreator(_context).Create();
            new DefaultOutFormatsCreator(_context).Create();
            new DefaultReportsCreator(_context).Create();
        }
    }
}
