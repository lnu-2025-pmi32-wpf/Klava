namespace Klava.Domain.Entities;

using Klava.Domain.Enums;

public class Subject
{
    public int Id { get; set; }
    
    public int TeamId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? SubjectInfo { get; set; }
    
    public SubjectStatus Status { get; set; } = SubjectStatus.Exam;
    
    public Team Team { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
