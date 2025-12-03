namespace Klava.Application.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? OwnerId { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
}
