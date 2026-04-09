namespace VMS.Core.Entities;

public class DocumentBlob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public Document? Document { get; set; }
}
