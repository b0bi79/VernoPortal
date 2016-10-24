using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Abp.Domain.Entities;

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
            name = NormalizeRegex.Replace(name, "_");
            int len;
            do
            {
                len = name.Length;
                name = name.Replace("__", "_");
            } while (name.Length != len);
            name = name.Trim('_');
            return Transliteration.CyrillicToLatin(name.ToLower());
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

        public ICollection<ReportFavorite> Favorites { get; set; }

        public ICollection<RepParameter> Parameters { get; set; }

        public ReportConnection Connection { get; set; }

        public ICollection<ReportOutFormat> OutFormats { get; set; }

        public ICollection<ReportColumn> Columns { get; set; }

        public ICollection<ReportResult> Results { get; set; }
    }
}