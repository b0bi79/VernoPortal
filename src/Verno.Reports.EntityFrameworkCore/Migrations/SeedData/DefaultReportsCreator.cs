using System.Linq;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.Migrations.SeedData
{
    public class DefaultReportsCreator
    {
        private readonly ReportsDbContext _context;

        public DefaultReportsCreator(ReportsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            AddReportIfNotExists("Отгрузка", "Комплектация по заказам", "INFOR_SPB", "OrderComplectation.sql", null);
            AddReportIfNotExists("Отгрузка", "Отчёт за вчера", "ION10MAIN_Mig", null, "Report_DayTurnover");
            AddReportIfNotExists("Загрузка", "Примеры параметров", "ION10MAIN_Mig", "ParametersTest.sql", null);
        }

        private void AddReportIfNotExists(string category, string name, string connectionId, string sqlfile, string sqlproc)
        {
            var report = _context.Reports.FirstOrDefault(s => s.Name == name && s.Category == category && s.SqlFile == sqlfile
                                                && s.SqlProc == sqlproc && s.Connection.Id == connectionId);
            if (report == null)
            {
                report = _context.Reports.Add(new Report(category, name, connectionId, sqlfile, sqlproc)).Entity;
                _context.SaveChanges();
            }
        }
    }
}