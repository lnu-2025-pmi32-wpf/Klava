namespace Klava.Domain.Entities;

public class SubjectFile
{
    public int Id { get; set; }
    
    public int SubjectId { get; set; }
    
    public string DisplayName { get; set; } = string.Empty;
    
    public string StorageName { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
    
    public long Size { get; set; }
    
    public DateTime UploadedAt { get; set; }
    
    public Subject Subject { get; set; } = null!;
}
