using System.ComponentModel.DataAnnotations.Schema;

namespace Verno.Reports.Models
{
    [Table("ReportFavorites")]
    public class ReportFavorite
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }
    }
}