using System.Linq;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.Migrations.SeedData
{
    public class DefaultOutFormatsCreator
    {
        private readonly ReportsDbContext _context;

        public DefaultOutFormatsCreator(ReportsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            AddFormatIfNotExists("HTML_TPL", "В Html", "XslTransform", null, "HTML", null);
            AddFormatIfNotExists("XLS_AUTO", "В Excel", "ImportFromSql", null, "XLS", null);
        }

        private void AddFormatIfNotExists(string id, string displayText, string generateUtil, string mimeType, string arguments, string description)
        {
            var format = _context.OutFormats.FirstOrDefault(s => s.Id == id);
            if (format != null) return;
            _context.OutFormats.Add(new OutFormat(id, displayText, generateUtil, mimeType, arguments, description));
            _context.SaveChanges();
        }
    }
}