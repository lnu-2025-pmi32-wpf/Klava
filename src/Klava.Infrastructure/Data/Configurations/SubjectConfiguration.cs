namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;
using Klava.Domain.Enums;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.TeamId)
            .HasColumnName("team_id");
            
        builder.Property(e => e.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(e => e.SubjectInfo)
            .HasColumnName("subject_info");
            
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion(
                v => v.ToString().ToLower(),
                v => (SubjectStatus)Enum.Parse(typeof(SubjectStatus), v, true))
            .HasColumnType("subject_status");
        
        builder.HasOne(e => e.Team)
            .WithMany(t => t.Subjects)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
