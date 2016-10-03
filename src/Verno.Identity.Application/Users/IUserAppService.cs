using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Verno.Identity.Users.Dto;

namespace Verno.Identity.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task ProhibitPermission(int userId, string permissionName);

        Task UpdateRoles(int userId, string[] roleNames);
        Task PasswordReset(int userId, PasswordResetInput input);

        ListResultOutput<UserDto> GetAll();
        Task<ListResultOutput<string>> GetRoles(int userId);

        Task<UserDto> Create(UserDto input);
        Task<UserDto> Update(UserDto user);
        Task<UserDto> Delete(int id);
    }
}