using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.EntityFrameworkCore
{
    public class ReportsDbContext : AbpDbContext
    {
        public DbSet<Report> Reports { get; set; }
        public DbSet<OutFormat> OutFormats { get; set; }
        public DbSet<ReportConnection> ReportConnections { get; set; }
        public DbSet<ReportFavorite> ReportFavorites { get; set; }
        public DbSet<ReportOutFormat> ReportOutFormats { get; set; }
        public DbSet<RepParameter> RepParameters { get; set; }
        public DbSet<ReportColumn> ReportColumns { get; set; }
        public DbSet<ReportResult> ReportResults { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public ReportsDbContext(DbContextOptions<ReportsDbContext> options) 
            : base(options)
        {

        }

        #region Overrides of AbpDbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ReportFavorite>().HasKey(x => new {x.ReportId, x.UserId});
            builder.Entity<ReportOutFormat>(b =>
            {
                b.HasOne(rf => rf.Report)
                    .WithMany(r => r.ReportOutFormats)
                    .HasForeignKey(rf => rf.ReportId);
                b.HasOne(rf => rf.OutFormat)
                    .WithMany()
                    .HasForeignKey(rf => rf.OutFormatId);
            });
            builder.Entity<ReportResult>().HasKey(r => new {r.ReportId, r.TableNo});
            builder.Entity<Report>().HasMany(r => r.Results).WithOne().HasForeignKey(r => r.ReportId);
        }

        #endregion
    }
}
