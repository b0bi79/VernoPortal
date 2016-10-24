using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Verno.Identity.Data;
using Verno.Identity.Migrations.SeedData;
using Verno.Identity.Permissions;
using Verno.Identity.Roles;
using Verno.Identity.Users;
using Verno.Reports.Authorization;

namespace Verno.Reports.Tests.TestDatas
{
    public static class TestIdentityBuilder
    {
        public static void Build(this IdentityDbContext context)
        {
            InitialHostDbBuilder.Build(context);

            var role1 = CreateRole(context, "role1", new [] { PermissionNames.Reports });
            CreateUser(context, "user1", "User 1", "user1@ivoin.ru", role1);
        }

        private static Role CreateRole(IdentityDbContext context, string name, IEnumerable<string> permissions)
        {
            var result = context.Roles.FirstOrDefault(r => r.Name == name);
            if (result == null)
            {
                result = context.Roles.Add(new Role { Name = name, }).Entity;
                context.SaveChanges();

                foreach (var permission in permissions)
                {
                    context.Permissions.Add(
                        new RolePermissionSetting
                        {
                            Name = permission,
                            IsGranted = true,
                            RoleId = result.Id
                        });
                }

                context.SaveChanges();
            }
            return result;
        }

        private static void CreateUser(IdentityDbContext context, string userName, string name, string email, Role role)
        {
            var result = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (result == null)
            {
                var user = new User(userName, name, email)
                {
                    EmailConfirmed = true,
                    IsActive = true,
                };
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, User.DefaultPassword);
                result = context.Users.Add(user).Entity;

                context.SaveChanges();

                context.UserRoles.Add(new UserRole { UserId = result.Id, RoleId = role.Id });

                context.SaveChanges();
            }
        }
    }
}
