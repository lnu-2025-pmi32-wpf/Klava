namespace Klava.Domain.Entities;

using Klava.Domain.Enums;

public class TeamMember
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    
    public TeamMemberRole Role { get; set; } = TeamMemberRole.Student;
    
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}
