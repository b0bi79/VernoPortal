using Abp.Application.Services.Dto;

namespace Verno.Identity.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public UserLoginInfoDto User { get; set; }
    }
}