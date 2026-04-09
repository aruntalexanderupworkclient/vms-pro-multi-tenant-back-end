namespace VMS.Core.Entities;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; } = true;

    public MdmLocationType? Type { get; set; }
    public Location? Parent { get; set; }
    public ICollection<Location> Children { get; set; } = new List<Location>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
