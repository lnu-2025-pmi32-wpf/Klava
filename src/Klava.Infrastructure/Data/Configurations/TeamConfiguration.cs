namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(8)
            .IsRequired();
            
        builder.Property(e => e.OwnerId)
            .HasColumnName("owner_id");
        
        builder.HasIndex(e => e.Code).IsUnique();
        
        builder.HasOne(e => e.Owner)
            .WithMany(u => u.OwnedTeams)
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
