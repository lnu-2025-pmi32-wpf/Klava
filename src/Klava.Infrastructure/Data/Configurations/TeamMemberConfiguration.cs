namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;
using Klava.Domain.Enums;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("teammembers");
        
        builder.HasKey(e => new { e.TeamId, e.UserId });
        
        builder.Property(e => e.TeamId)
            .HasColumnName("team_id");
            
        builder.Property(e => e.UserId)
            .HasColumnName("user_id");
            
        builder.Property(e => e.Role)
            .HasColumnName("role")
            .HasConversion(
                v => v.ToString().ToLower(),
                v => (TeamMemberRole)Enum.Parse(typeof(TeamMemberRole), v, true))
            .HasColumnType("team_member_role");
        
        builder.HasOne(e => e.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
