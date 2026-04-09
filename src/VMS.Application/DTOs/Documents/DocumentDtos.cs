namespace VMS.Application.DTOs.Documents;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public Guid? EntityTypeId { get; set; }
    public string? EntityTypeName { get; set; }
    public Guid? FileTypeId { get; set; }
    public string? FileTypeName { get; set; }
    public Guid? EntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UploadDocumentDto
{
    public Guid? EntityTypeId { get; set; }
    public Guid? FileTypeId { get; set; }
    public Guid? EntityId { get; set; }
    public string? Description { get; set; }
}
