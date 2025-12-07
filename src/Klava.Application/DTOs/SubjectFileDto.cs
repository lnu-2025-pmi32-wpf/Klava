namespace Klava.Application.DTOs;

public class SubjectFileDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
