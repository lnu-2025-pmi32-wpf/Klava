namespace Klava.Domain.Entities;

public class Task
{
    public int Id { get; set; }
    
    public int SubjectId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime? Deadline { get; set; }
    
    // Navigation properties
    public Subject Subject { get; set; } = null!;
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
