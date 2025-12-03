namespace Klava.Application.DTOs;

public class CreateTaskRequest
{
    public int SubjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}
