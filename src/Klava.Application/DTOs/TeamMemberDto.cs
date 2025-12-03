namespace Klava.Application.DTOs;

using Klava.Domain.Enums;

public class TeamMemberDto
{
    public int UserId { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public TeamMemberRole Role { get; set; }
}
