namespace VMS.Application.DTOs.Locations;

public class LocationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TypeId { get; set; }
    public string? TypeName { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public List<LocationDto> Children { get; set; } = new();
}

public class CreateLocationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ParentId { get; set; }
}

public class UpdateLocationDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ParentId { get; set; }
    public bool? IsActive { get; set; }
}
