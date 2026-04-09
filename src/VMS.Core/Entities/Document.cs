namespace VMS.Core.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public Guid? EntityTypeId { get; set; }
    public Guid? FileTypeId { get; set; }
    public Guid? EntityId { get; set; }
    public string? Description { get; set; }

    public MdmEntityType? EntityType { get; set; }
    public MdmFileType? FileType { get; set; }
    public DocumentBlob? Blob { get; set; }
}
