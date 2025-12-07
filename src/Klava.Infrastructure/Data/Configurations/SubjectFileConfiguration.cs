namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;

public class SubjectFileConfiguration : IEntityTypeConfiguration<SubjectFile>
{
    public void Configure(EntityTypeBuilder<SubjectFile> builder)
    {
        builder.ToTable("subjectfiles");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.SubjectId)
            .HasColumnName("subject_id");
            
        builder.Property(e => e.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(e => e.StorageName)
            .HasColumnName("storage_name")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(e => e.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(e => e.Size)
            .HasColumnName("size");
            
        builder.Property(e => e.UploadedAt)
            .HasColumnName("uploaded_at");
        
        builder.HasOne(e => e.Subject)
            .WithMany(s => s.Files)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
