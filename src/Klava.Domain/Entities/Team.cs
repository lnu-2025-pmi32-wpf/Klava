namespace Klava.Domain.Entities;

public class Team
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty; 
    
    public int? OwnerId { get; set; }
    
    public User? Owner { get; set; }
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
