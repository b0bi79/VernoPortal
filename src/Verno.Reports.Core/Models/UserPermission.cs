using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    [Table("UserPermissions")]
    public class UserPermission: Entity<int>
    {
        public UserPermission()
        {
        }

        public UserPermission(int reportId, int userId)
        {
            ReportId = reportId;
            UserId = userId;
        }

        public int ReportId { get; set; }
        public int UserId { get; set; }
    }
}