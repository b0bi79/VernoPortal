using System.Linq;
using Abp.Authorization;
using Abp.MultiTenancy;
using Verno.Identity.Authorization;
using Verno.Identity.Roles;
using Verno.Identity.Data;
using Verno.Identity.Users;
using Microsoft.AspNetCore.Identity;

namespace Verno.Identity.Migrations.SeedData
{
    public class HostRoleAndUserCreator
    {
        private readonly IdentityDbContext _context;

        public HostRoleAndUserCreator(IdentityDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            //Admin role for host

            var adminRoleForHost = _context.Roles.FirstOrDefault(r => r.Name == StaticRoleNames.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = _context.Roles.Add(new Role { Name = StaticRoleNames.Admin, }).Entity;
                _context.SaveChanges();

                //Grant all tenant permissions
                var permissions = PermissionFinder
                    .GetAllPermissions(new IdentityAuthorizationProvider())
                    .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host))
                    .ToList();

                foreach (var permission in permissions)
                {
                    _context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            Name = permission.Name,
                            IsGranted = true,
                            RoleId = adminRoleForHost.Id
                        });
                }

                _context.SaveChanges();
            }

            //Admin user for tenancy host

            var adminUserForHost = _context.Users.FirstOrDefault(u => u.UserName == User.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User(User.AdminUserName, "Administrator", "admin@ivoin.ru")
                {
                    EmailConfirmed = true,
                    IsActive = true,
                };
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, User.DefaultPassword);
                adminUserForHost = _context.Users.Add(user).Entity;

                _context.SaveChanges();

                _context.UserRoles.Add(new UserRole {UserId = adminUserForHost.Id, RoleId = adminRoleForHost.Id});

                _context.SaveChanges();
            }
        }
    }
}