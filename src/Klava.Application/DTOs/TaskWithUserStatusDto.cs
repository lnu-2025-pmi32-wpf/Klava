namespace Klava.Application.DTOs;

using Klava.Domain.Enums;

public class TaskWithUserStatusDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectTitle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    
    // User-specific status
    public SubmissionStatus? CurrentUserStatus { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
