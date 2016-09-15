using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Verno.Identity.Settings;

namespace Verno.Identity.Data
{
    using Organizations;
    using Permissions;
    using Roles;
    using Users;

    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected IdentityDbContext()
        {
        }

        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<UserLoginAttempt> LoginAttempts { get; set; }
        public DbSet<PermissionSetting> Permissions { get; set; }
        public DbSet<RolePermissionSetting> RolePermissions { get; set; }
        public DbSet<UserPermissionSetting> UserPermissions { get; set; }
        public DbSet<ClaimPermissionSetting> ClaimPermissions { get; set; }

         /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">
        /// The builder being used to construct the model for this context.
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // This needs to go before the other rules!

            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.ToTable("Users");
                b.Property(u => u.IsActive).HasDefaultValue(true);
                b.Property(u => u.AccessFailedCount).HasDefaultValue(0);
                b.Property(u => u.EmailConfirmed).HasDefaultValue(false);
                b.Property(u => u.LockoutEnabled).HasDefaultValue(false);
                b.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);
                b.Property(u => u.TwoFactorEnabled).HasDefaultValue(false);

                b.HasOne(u => u.OrgUnit).WithMany(o => o.Users).OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Role>(b =>
            {
                b.Property(c => c.Application).HasMaxLength(50);
                b.ToTable("Roles");
            });
            builder.Entity<UserClaim>(b =>
            {
                b.ToTable("UserClaims");
                b.Property(c => c.Application).HasMaxLength(50);
            });
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            builder.Entity<Setting>(b =>
            {
                b.HasKey(s => s.Id);
                b.HasIndex(s => s.Name).HasName("NameIndex").IsUnique();
                b.HasIndex(s => s.UserId).HasName("UserIdIndex");
                b.ToTable("Settings");

                b.Property(s => s.Name).HasMaxLength(Setting.MaxNameLength);
                b.Property(s => s.Value).HasMaxLength(Setting.MaxValueLength);
            });

            builder.Entity<UserLoginAttempt>(b =>
            {
                b.ToTable("UserLoginAttempts");
                b.Property(c => c.Application).HasMaxLength(50);
            });

            builder.Entity<OrgUnit>(b =>
            {
                b.HasKey(s => s.Id);
                b.ToTable("OrgUnits");
            });
        }
    }
}