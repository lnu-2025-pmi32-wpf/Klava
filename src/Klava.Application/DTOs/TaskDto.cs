namespace Klava.Application.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectTitle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
}
