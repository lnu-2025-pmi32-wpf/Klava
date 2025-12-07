namespace Klava.Domain.Entities;

using Klava.Domain.Enums;

public class Submission
{
    public int Id { get; set; }
    
    public int TaskId { get; set; }
    
    public int UserId { get; set; }
    
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Wait;
    
    public DateTime SubmittedAt { get; set; }
    
    // Navigation properties
    public Task Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
