using Microsoft.EntityFrameworkCore;
using VMS.Core.Entities;
using VMS.Shared.Constants;
using VMS.Shared.Helpers;

namespace VMS.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(VmsDbContext context)
    {
        if (await context.Tenants.AnyAsync())
            return;

        var tenantId = Guid.NewGuid();

        // 1. Tenant
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "VMS Corp",
            Code = "VMSCORP",
            IsActive = true
        };
        context.Tenants.Add(tenant);

        // 2. Roles
        var superAdminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = AppConstants.Roles.SuperAdmin,
            Description = "Full system access",
            IsAdmin = true,
            TenantId = tenantId
        };
        var frontDeskRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = AppConstants.Roles.FrontDesk,
            Description = "Front desk operations",
            IsAdmin = false,
            TenantId = tenantId
        };
        var securityRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = AppConstants.Roles.Security,
            Description = "Security operations",
            IsAdmin = false,
            TenantId = tenantId
        };
        context.Roles.AddRange(superAdminRole, frontDeskRole, securityRole);

        // 3. Users
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Admin",
            Email = "admin@vms.com",
            PasswordHash = PasswordHasher.Hash("Admin@123"),
            RoleId = superAdminRole.Id,
            TenantId = tenantId,
            IsActive = true
        };
        var frontDeskUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Front Desk User",
            Email = "frontdesk@vms.com",
            PasswordHash = PasswordHasher.Hash("FrontDesk@123"),
            RoleId = frontDeskRole.Id,
            TenantId = tenantId,
            IsActive = true
        };
        var securityUser = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Security User",
            Email = "security@vms.com",
            PasswordHash = PasswordHasher.Hash("Security@123"),
            RoleId = securityRole.Id,
            TenantId = tenantId,
            IsActive = true
        };
        context.Users.AddRange(adminUser, frontDeskUser, securityUser);

        // 4. MDM Seed Data
        SeedMdmData(context, tenantId);

        // 4b. Locations
        var location1 = new Location { Id = Guid.NewGuid(), Name = "Main Tower", Code = "MAIN-TWR", Level = 0, IsActive = true, TenantId = tenantId };
        var location2 = new Location { Id = Guid.NewGuid(), Name = "Floor 1", Code = "FLR-1", ParentId = location1.Id, Level = 1, IsActive = true, TenantId = tenantId };
        var location3 = new Location { Id = Guid.NewGuid(), Name = "Conference Room A", Code = "CONF-A", ParentId = location2.Id, Level = 2, IsActive = true, TenantId = tenantId };
        context.Locations.AddRange(location1, location2, location3);

        // 4c. Hosts
        var host1 = new HostPerson
        {
            Id = Guid.NewGuid(),
            FullName = "John Smith",
            Email = "john.smith@vms.com",
            Phone = "+1-555-0101",
            Department = "Engineering",
            Designation = "Tech Lead",
            LocationId = location2.Id,
            UserId = adminUser.Id,
            IsActive = true,
            TenantId = tenantId
        };
        var host2 = new HostPerson
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Doe",
            Email = "jane.doe@vms.com",
            Phone = "+1-555-0102",
            Department = "HR",
            Designation = "HR Manager",
            LocationId = location2.Id,
            IsActive = true,
            TenantId = tenantId
        };
        context.Hosts.AddRange(host1, host2);

        // 4d. Visitors
        var visitor1 = new Visitor
        {
            Id = Guid.NewGuid(),
            FullName = "Alice Johnson",
            Email = "alice.johnson@example.com",
            Phone = "+1-555-0201",
            Company = "Acme Corp",
            IdProofType = "Passport",
            IdProofNumber = "P1234567",
            TenantId = tenantId
        };
        var visitor2 = new Visitor
        {
            Id = Guid.NewGuid(),
            FullName = "Bob Williams",
            Email = "bob.williams@example.com",
            Phone = "+1-555-0202",
            Company = "Globex Inc",
            IdProofType = "Driver License",
            IdProofNumber = "DL9876543",
            TenantId = tenantId
        };
        var visitor3 = new Visitor
        {
            Id = Guid.NewGuid(),
            FullName = "Charlie Brown",
            Email = "charlie.brown@example.com",
            Phone = "+1-555-0203",
            Company = "Initech",
            IdProofType = "National ID",
            IdProofNumber = "NID1122334",
            TenantId = tenantId
        };
        context.Visitors.AddRange(visitor1, visitor2, visitor3);

        // 4e. Visits
        var now = DateTime.UtcNow;
        var visit1 = new Visit
        {
            Id = Guid.NewGuid(),
            VisitorId = visitor1.Id,
            HostId = host1.Id,
            LocationId = location3.Id,
            Purpose = "Project kickoff meeting",
            Status = Core.Enums.VisitStatus.CheckedIn,
            ScheduledDateTime = now.AddHours(-1),
            CheckInTime = now.AddMinutes(-45),
            AccessCardNumber = "AC-001",
            AccessCardIssuedAt = now.AddMinutes(-45),
            TenantId = tenantId
        };
        var visit2 = new Visit
        {
            Id = Guid.NewGuid(),
            VisitorId = visitor2.Id,
            HostId = host2.Id,
            LocationId = location2.Id,
            Purpose = "Interview",
            Status = Core.Enums.VisitStatus.CheckedOut,
            ScheduledDateTime = now.AddDays(-1),
            CheckInTime = now.AddDays(-1).AddMinutes(10),
            CheckOutTime = now.AddDays(-1).AddHours(2),
            AccessCardNumber = "AC-002",
            AccessCardIssuedAt = now.AddDays(-1).AddMinutes(10),
            AccessCardReturnedAt = now.AddDays(-1).AddHours(2),
            Remarks = "Completed successfully",
            TenantId = tenantId
        };
        var visit3 = new Visit
        {
            Id = Guid.NewGuid(),
            VisitorId = visitor3.Id,
            HostId = host1.Id,
            LocationId = location3.Id,
            Purpose = "Vendor demo",
            Status = Core.Enums.VisitStatus.Scheduled,
            ScheduledDateTime = now.AddDays(1),
            TenantId = tenantId
        };
        var visit4 = new Visit
        {
            Id = Guid.NewGuid(),
            VisitorId = visitor1.Id,
            HostId = host2.Id,
            Purpose = "Follow-up discussion",
            Status = Core.Enums.VisitStatus.Scheduled,
            ScheduledDateTime = now.AddDays(2),
            TenantId = tenantId
        };
        context.Visits.AddRange(visit1, visit2, visit3, visit4);

        // 5. Menus
        var menus = GetMenuSeedData(tenantId);
        context.Menus.AddRange(menus);

        // 6. RolePermissions — SuperAdmin gets full access to all menus
        foreach (var menu in menus)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                MenuId = menu.Id,
                TenantId = tenantId,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
                CanPrint = true
            });
        }

        // 6b. RolePermissions — FrontDesk gets access to Dashboard, Visitors, Locations, Reports
        var menuLookup = menus.ToDictionary(m => m.Name);
        var frontDeskMenuPermissions = new Dictionary<string, (bool Create, bool Read, bool Update, bool Delete, bool Print)>
        {
            ["Dashboard"] = (false, true, false, false, false),
            ["Visitors"]  = (true, true, true, false, true),
            ["Locations"] = (false, true, false, false, false),
            ["Reports"]   = (false, true, false, false, true)
        };
        foreach (var (menuName, perms) in frontDeskMenuPermissions)
        {
            if (menuLookup.TryGetValue(menuName, out var menu))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = frontDeskRole.Id,
                    MenuId = menu.Id,
                    TenantId = tenantId,
                    CanCreate = perms.Create,
                    CanRead = perms.Read,
                    CanUpdate = perms.Update,
                    CanDelete = perms.Delete,
                    CanPrint = perms.Print
                });
            }
        }

        // 6c. RolePermissions — Security gets read-only access to Dashboard and Visitors
        var securityMenuPermissions = new Dictionary<string, (bool Create, bool Read, bool Update, bool Delete, bool Print)>
        {
            ["Dashboard"] = (false, true, false, false, false),
            ["Visitors"]  = (false, true, false, false, false)
        };
        foreach (var (menuName, perms) in securityMenuPermissions)
        {
            if (menuLookup.TryGetValue(menuName, out var menu))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = securityRole.Id,
                    MenuId = menu.Id,
                    TenantId = tenantId,
                    CanCreate = perms.Create,
                    CanRead = perms.Read,
                    CanUpdate = perms.Update,
                    CanDelete = perms.Delete,
                    CanPrint = perms.Print
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static void SeedMdmData(VmsDbContext context, Guid tenantId)
    {
        // Tenant Types
        context.MdmTenantTypes.AddRange(
            new MdmTenantType { Name = "Enterprise", Code = AppConstants.MdmValueCodes.Enterprise, DisplayOrder = 1, TenantId = tenantId },
            new MdmTenantType { Name = "SMB", Code = AppConstants.MdmValueCodes.SMB, DisplayOrder = 2, TenantId = tenantId }
        );

        // Plan Types
        context.MdmPlanTypes.AddRange(
            new MdmPlanType { Name = "Basic", Code = AppConstants.MdmValueCodes.Basic, DisplayOrder = 1, TenantId = tenantId },
            new MdmPlanType { Name = "Pro", Code = AppConstants.MdmValueCodes.Pro, DisplayOrder = 2, TenantId = tenantId }
        );

        // Location Types
        context.MdmLocationTypes.AddRange(
            new MdmLocationType { Name = "Tower", Code = AppConstants.MdmValueCodes.Tower, DisplayOrder = 1, TenantId = tenantId },
            new MdmLocationType { Name = "Floor", Code = AppConstants.MdmValueCodes.Floor, DisplayOrder = 2, TenantId = tenantId },
            new MdmLocationType { Name = "Room", Code = AppConstants.MdmValueCodes.Room, DisplayOrder = 3, TenantId = tenantId },
            new MdmLocationType { Name = "Department", Code = AppConstants.MdmValueCodes.Department, DisplayOrder = 4, TenantId = tenantId }
        );

        // File Types
        context.MdmFileTypes.AddRange(
            new MdmFileType { Name = "PDF", Code = AppConstants.MdmValueCodes.Pdf, DisplayOrder = 1, TenantId = tenantId },
            new MdmFileType { Name = "Image", Code = AppConstants.MdmValueCodes.Image, DisplayOrder = 2, TenantId = tenantId },
            new MdmFileType { Name = "Document", Code = AppConstants.MdmValueCodes.Doc, DisplayOrder = 3, TenantId = tenantId }
        );

        // Entity Types
        context.MdmEntityTypes.AddRange(
            new MdmEntityType { Name = "Visitor", Code = AppConstants.MdmValueCodes.VisitorEntity, DisplayOrder = 1, TenantId = tenantId },
            new MdmEntityType { Name = "User", Code = AppConstants.MdmValueCodes.UserEntity, DisplayOrder = 2, TenantId = tenantId },
            new MdmEntityType { Name = "Host", Code = AppConstants.MdmValueCodes.HostEntity, DisplayOrder = 3, TenantId = tenantId }
        );
    }

    private static List<Menu> GetMenuSeedData(Guid tenantId)
    {
        return new List<Menu>
        {
            new() { Id = Guid.NewGuid(), Name = "Dashboard", Icon = "dashboard", Route = "/dashboard", DisplayOrder = 1, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Visitors", Icon = "people", Route = "/visitors", DisplayOrder = 2, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Users", Icon = "person", Route = "/users", DisplayOrder = 3, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Roles", Icon = "security", Route = "/roles", DisplayOrder = 4, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Locations", Icon = "location_on", Route = "/locations", DisplayOrder = 5, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Settings", Icon = "settings", Route = "/settings", DisplayOrder = 6, TenantId = tenantId },
            new() { Id = Guid.NewGuid(), Name = "Reports", Icon = "assessment", Route = "/reports", DisplayOrder = 7, TenantId = tenantId }
        };
    }

}
