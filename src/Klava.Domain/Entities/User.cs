namespace Klava.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string Firstname { get; set; } = string.Empty;
    
    public string Lastname { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty; 
    
    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public ICollection<Team> OwnedTeams { get; set; } = new List<Team>();
}
