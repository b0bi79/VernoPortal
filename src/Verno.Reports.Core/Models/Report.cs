using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Abp.Domain.Entities;
using System.Linq;

namespace Verno.Reports.Models
{
    [Table("Reports")]
    public class Report : Entity<int>
    {
        private static readonly Regex NormalizeRegex = new Regex("[^-0-9a-zA-Zа-яА-Я]");
        private string _name;

        public Report()
        {
        }

        /// <inheritdoc />
        public Report(string category, string name, string connectionId, string sqlFile, string sqlProc)
        {
            Category = category;
            ConnectionId = connectionId;
            Name = name;
            SqlFile = sqlFile;
            SqlProc = sqlProc;
        }

        protected string NormalizeName(string name)
        {
            name = Transliteration.CyrillicToLatin(name.ToLower());
            name = NormalizeRegex.Replace(name, "_");
            int len;
            do
            {
                len = name.Length;
                name = name.Replace("__", "_");
            } while (name.Length != len);
            name = name.Trim('_');
            return name;
        }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NormalizedName = NormalizeName(value);
            }
        }

        public string NormalizedName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ConnectionId { get; set; }
        public string SqlFile { get; set; }
        public string SqlProc { get; set; }

        public ICollection<ReportFavorite> Favorites { get; set; } = new List<ReportFavorite>();

        public ICollection<RepParameter> Parameters { get; set; } = new List<RepParameter>();

        public ReportConnection Connection { get; set; }

        public ICollection<ReportOutFormat> ReportOutFormats { get; set; } = new List<ReportOutFormat>();

        public ICollection<ReportColumn> Columns { get; set; } = new List<ReportColumn>();

        public IList<ReportResult> Results { get; set; } = new List<ReportResult>();

        public string GetTableName(int idx)
        {
            var res = Results.FirstOrDefault(x => x.TableNo == idx);
            return res != null ? res.Title : Name;
        }
    }
}