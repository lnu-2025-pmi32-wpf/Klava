namespace Klava.DataSeeder.Models;

public enum TeamMemberRole
{
    Student,
    Headman
}

public class TeamMember
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public TeamMemberRole Role { get; set; }
}

