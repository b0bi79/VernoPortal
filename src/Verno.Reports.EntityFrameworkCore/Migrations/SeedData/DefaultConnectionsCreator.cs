using System.Linq;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.Migrations.SeedData
{
    public class DefaultConnectionsCreator
    {
        private readonly ReportsDbContext _context;

        public DefaultConnectionsCreator(ReportsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            AddConnectionIfNotExists("INFOR_SPB", "SqlServer", "Server = 10.53.2.21; Database = SCPRD; Uid = wmwhse1; Pwd = WMwhSql1;");
            AddConnectionIfNotExists("ION10MAIN_Eff", "SqlServer.2005", "Server = 10.49.40.10\\MAIN; Database = shEffSQL; user = dan; password = dan");
            AddConnectionIfNotExists("ION10MAIN_Mig", "SqlServer.2005", "Server = 10.49.40.10\\MAIN; Database = shMigSQL; user = dan; password = dan");
        }

        private void AddConnectionIfNotExists(string id, string providerName, string connectionString)
        {
            var connection = _context.ReportConnections.FirstOrDefault(s => s.Id == id);
            if (connection != null) return;

            _context.ReportConnections.Add(new ReportConnection(id, providerName, connectionString));
            _context.SaveChanges();
        }
    }
}