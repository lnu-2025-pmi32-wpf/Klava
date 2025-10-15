namespace Klava.DataSeeder.Models;

public enum SubmissionStatus
{
    Done,
    Wait
}

public class Submission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public SubmissionStatus Status { get; set; }
}

