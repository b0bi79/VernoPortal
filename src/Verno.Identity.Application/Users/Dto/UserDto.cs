using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Identity.OrgUnits.Dto;

namespace Verno.Identity.Users.Dto
{
    [AutoMap(typeof(User))]
    public class UserDto : EntityDto
    {
        [Required]
        [Display(Name = "Subdivision")]
        public int OrgUnitId { get; set; }
        public OrgUnitDto OrgUnit { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string BossId { get; set; }
        public string Position { get; set; }
        public int? ShopNum { get; set; }
    }
}