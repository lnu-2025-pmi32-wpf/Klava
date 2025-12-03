namespace Klava.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Klava.Domain.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.Property(e => e.Firstname)
            .HasColumnName("firstname")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(e => e.Lastname)
            .HasColumnName("lastname")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(e => e.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();
    }
}
