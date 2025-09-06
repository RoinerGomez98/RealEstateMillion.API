using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Infrastructure.Data.Configurations
{
    public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
    {
        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder.ToTable("Owners");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Photo)
                .HasMaxLength(500);

            builder.Property(o => o.Phone)
                .HasMaxLength(20);

            builder.Property(o => o.Email)
                .HasMaxLength(100);

            builder.Property(o => o.DocumentType)
                .HasMaxLength(50);

            builder.Property(o => o.DocumentNumber)
                .HasMaxLength(50);

            builder.Property(o => o.City)
                .HasMaxLength(100);

            builder.Property(o => o.State)
                .HasMaxLength(50);

            builder.Property(o => o.ZipCode)
                .HasMaxLength(10);

            builder.Property(o => o.Country)
                .HasMaxLength(50)
                .HasDefaultValue("USA");

            builder.Property(o => o.CreatedBy)
                .HasMaxLength(100);

            builder.Property(o => o.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(o => o.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

            builder.HasIndex(o => new { o.DocumentType, o.DocumentNumber })
                .IsUnique()
                .HasFilter("[DocumentType] IS NOT NULL AND [DocumentNumber] IS NOT NULL");

            builder.HasIndex(o => o.Name);

            builder.HasMany(o => o.Properties)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
