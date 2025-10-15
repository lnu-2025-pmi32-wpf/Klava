namespace Klava.DataSeeder.Models;

public enum SubjectStatus
{
    Exam,
    Test
}

public class Subject
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectInfo { get; set; } = string.Empty;
    public SubjectStatus Status { get; set; }
}

