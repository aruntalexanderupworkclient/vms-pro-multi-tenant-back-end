using Microsoft.EntityFrameworkCore;
using VMS.Application.Interfaces;
using VMS.Core.Entities;

namespace VMS.Infrastructure.Data;

public class VmsDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public VmsDbContext(DbContextOptions<VmsDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<HostPerson> Hosts => Set<HostPerson>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<MdmTenantType> MdmTenantTypes => Set<MdmTenantType>();
    public DbSet<MdmPlanType> MdmPlanTypes => Set<MdmPlanType>();
    public DbSet<MdmLocationType> MdmLocationTypes => Set<MdmLocationType>();
    public DbSet<MdmFileType> MdmFileTypes => Set<MdmFileType>();
    public DbSet<MdmEntityType> MdmEntityTypes => Set<MdmEntityType>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentBlob> DocumentBlobs => Set<DocumentBlob>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // Property used by EF Core query filters — must be a property/field reference
    // so the filter re-evaluates on each query instead of capturing a local variable.
    private Guid TenantId => _tenantProvider.GetTenantId();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Tenant (no TenantId filter on itself) =====
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.HasQueryFilter(t => !t.IsDeleted);
        });

        // ===== User =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => new { u.Email, u.TenantId }).IsUnique();
            entity.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(u => u.Tenant).WithMany(t => t.Users).HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(u => u.TenantId == TenantId && !u.IsDeleted);
        });

        // ===== HostPerson =====
        modelBuilder.Entity<HostPerson>(entity =>
        {
            entity.ToTable("Hosts");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.FullName).IsRequired().HasMaxLength(200);
            entity.Property(h => h.Email).HasMaxLength(200);
            entity.HasOne(h => h.Location).WithMany().HasForeignKey(h => h.LocationId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(h => h.User).WithOne().HasForeignKey<HostPerson>(h => h.UserId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(h => h.Tenant).WithMany(t => t.Hosts).HasForeignKey(h => h.TenantId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(h => h.TenantId == TenantId && !h.IsDeleted);
        });

        // ===== Role =====
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(r => r.Tenant).WithMany(t => t.Roles).HasForeignKey(r => r.TenantId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(r => r.TenantId == TenantId && !r.IsDeleted);
        });

        // ===== Menu =====
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(m => m.Parent).WithMany(m => m.Children).HasForeignKey(m => m.ParentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(m => m.TenantId == TenantId && !m.IsDeleted);
        });

        // ===== RolePermission =====
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => rp.Id);
            entity.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(rp => rp.Menu).WithMany(m => m.RolePermissions).HasForeignKey(rp => rp.MenuId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(rp => rp.TenantId == TenantId && !rp.IsDeleted);
        });

        // ===== RefreshToken =====
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(rt => rt.TenantId == TenantId && !rt.IsDeleted);
        });

        // ===== Visitor =====
        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.FullName).IsRequired().HasMaxLength(200);
            entity.HasOne(v => v.Tenant).WithMany().HasForeignKey(v => v.TenantId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(v => v.TenantId == TenantId && !v.IsDeleted);
        });

        // ===== Visit =====
        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasOne(v => v.Visitor).WithMany(vis => vis.Visits).HasForeignKey(v => v.VisitorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(v => v.Host).WithMany(h => h.Visits).HasForeignKey(v => v.HostId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(v => v.Location).WithMany(l => l.Visits).HasForeignKey(v => v.LocationId).OnDelete(DeleteBehavior.SetNull);
            entity.HasQueryFilter(v => v.TenantId == TenantId && !v.IsDeleted);
        });

        // ===== Location =====
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(l => l.Type).WithMany().HasForeignKey(l => l.TypeId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(l => l.Parent).WithMany(l => l.Children).HasForeignKey(l => l.ParentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(l => l.TenantId == TenantId && !l.IsDeleted);
        });

        // ===== MdmTenantType =====
        modelBuilder.Entity<MdmTenantType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Code, e.TenantId }).IsUnique();
            entity.HasQueryFilter(e => e.TenantId == TenantId && !e.IsDeleted);
        });

        // ===== MdmPlanType =====
        modelBuilder.Entity<MdmPlanType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Code, e.TenantId }).IsUnique();
            entity.HasQueryFilter(e => e.TenantId == TenantId && !e.IsDeleted);
        });

        // ===== MdmLocationType =====
        modelBuilder.Entity<MdmLocationType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Code, e.TenantId }).IsUnique();
            entity.HasQueryFilter(e => e.TenantId == TenantId && !e.IsDeleted);
        });

        // ===== MdmFileType =====
        modelBuilder.Entity<MdmFileType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Code, e.TenantId }).IsUnique();
            entity.HasQueryFilter(e => e.TenantId == TenantId && !e.IsDeleted);
        });

        // ===== MdmEntityType =====
        modelBuilder.Entity<MdmEntityType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Code, e.TenantId }).IsUnique();
            entity.HasQueryFilter(e => e.TenantId == TenantId && !e.IsDeleted);
        });

        // ===== Document =====
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FileName).IsRequired().HasMaxLength(500);
            entity.HasOne(d => d.EntityType).WithMany().HasForeignKey(d => d.EntityTypeId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.FileType).WithMany().HasForeignKey(d => d.FileTypeId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(d => d.Blob).WithOne(b => b.Document!).HasForeignKey<DocumentBlob>(b => b.DocumentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasQueryFilter(d => d.TenantId == TenantId && !d.IsDeleted);
        });

        // ===== DocumentBlob =====
        modelBuilder.Entity<DocumentBlob>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Data).IsRequired();
        });

        // ===== Notification =====
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(2000);
            entity.HasOne(n => n.RecipientUser).WithMany().HasForeignKey(n => n.RecipientUserId).OnDelete(DeleteBehavior.SetNull);
            entity.HasQueryFilter(n => n.TenantId == TenantId && !n.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.TenantId == Guid.Empty)
                        entry.Entity.TenantId = tenantId;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
