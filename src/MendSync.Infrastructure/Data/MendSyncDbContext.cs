using MendSync.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MendSync.Infrastructure.Data;

public class MendSyncDbContext(DbContextOptions<MendSyncDbContext> options) : DbContext(options)
{
    public DbSet<MendApplication> Applications => Set<MendApplication>();
    public DbSet<MendProject> Projects => Set<MendProject>();
    public DbSet<MendLabel> Labels => Set<MendLabel>();
    public DbSet<MendUser> Users => Set<MendUser>();
    public DbSet<MendGroup> Groups => Set<MendGroup>();
    public DbSet<MendSyncLog> SyncLogs => Set<MendSyncLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MendApplication>(e =>
        {
            e.ToTable("MEND_APPLICATIONS");
            e.HasKey(x => x.Uuid);
            e.Property(x => x.Uuid).HasMaxLength(36);
            e.Property(x => x.Name).HasMaxLength(500).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<MendProject>(e =>
        {
            e.ToTable("MEND_PROJECTS");
            e.HasKey(x => x.Uuid);
            e.Property(x => x.Uuid).HasMaxLength(36);
            e.Property(x => x.Name).HasMaxLength(500).IsRequired();
            e.Property(x => x.ApplicationUuid).HasMaxLength(36);
            e.Property(x => x.ApplicationName).HasMaxLength(500);
        });

        modelBuilder.Entity<MendLabel>(e =>
        {
            e.ToTable("MEND_LABELS");
            e.HasKey(x => x.Uuid);
            e.Property(x => x.Uuid).HasMaxLength(36);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Color).HasMaxLength(50);
        });

        modelBuilder.Entity<MendUser>(e =>
        {
            e.ToTable("MEND_USERS");
            e.HasKey(x => x.Uuid);
            e.Property(x => x.Uuid).HasMaxLength(36);
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Role).HasMaxLength(100);
        });

        modelBuilder.Entity<MendGroup>(e =>
        {
            e.ToTable("MEND_GROUPS");
            e.HasKey(x => x.Uuid);
            e.Property(x => x.Uuid).HasMaxLength(36);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<MendSyncLog>(e =>
        {
            e.ToTable("MEND_SYNC_LOG");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
        });
    }
}
