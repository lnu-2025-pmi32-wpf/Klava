namespace Klava.Application.DTOs;

using Klava.Domain.Enums;

public class SubjectDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? SubjectInfo { get; set; }
    public SubjectStatus Status { get; set; }
}
