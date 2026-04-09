namespace VMS.Application.DTOs.Menus;

public class MenuDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<MenuDto> Children { get; set; } = new();
}

public class MenuWithPermissionDto
{
    public Guid MenuId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPrint { get; set; }
    public List<MenuWithPermissionDto> Children { get; set; } = new();
}

public class CreateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateMenuDto
{
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public Guid? ParentId { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsActive { get; set; }
}
