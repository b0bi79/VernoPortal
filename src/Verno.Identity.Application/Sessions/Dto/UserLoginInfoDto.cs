using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Identity.Users;

namespace Verno.Identity.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int OrgUnitId { get; set; }
    }
}
