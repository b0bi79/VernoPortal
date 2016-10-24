using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    [Table("RolePermissions")]
    public class RolePermission: Entity<int>
    {
        public int ReportId { get; set; }
        public string Role { get; set; }
    }
}