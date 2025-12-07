namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;
using Klava.Domain.Enums;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("submissions");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.TaskId)
            .HasColumnName("task_id");
            
        builder.Property(e => e.UserId)
            .HasColumnName("user_id");
            
        builder.Property(e => e.Status)
            .HasColumnName("status");
            
        builder.Property(e => e.SubmittedAt)
            .HasColumnName("submitted_at");
        
        // Composite unique constraint: one submission per user per task
        builder.HasIndex(e => new { e.TaskId, e.UserId })
            .IsUnique();
        
        // Relationship: Submission -> Task
        builder.HasOne(e => e.Task)
            .WithMany(t => t.Submissions)
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relationship: Submission -> User
        builder.HasOne(e => e.User)
            .WithMany(u => u.Submissions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
