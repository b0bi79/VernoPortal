using Abp.AutoMapper;
using Abp.Domain.Entities;

namespace Verno.Identity.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleDto: Entity<int>
    {
        public string Name { get; set; }
        public string Application { get; set; }
    }
}