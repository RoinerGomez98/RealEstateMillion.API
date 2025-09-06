using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Infrastructure.Data.Configurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.ToTable("PropertyImages");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.File)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pi => pi.Title)
                .HasMaxLength(200);

            builder.Property(pi => pi.Description)
                .HasMaxLength(500);

            builder.Property(pi => pi.FileType)
                .HasMaxLength(50);

            builder.Property(pi => pi.ThumbnailPath)
                .HasMaxLength(500);

            builder.Property(pi => pi.CreatedBy)
                .HasMaxLength(100);

            builder.Property(pi => pi.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(pi => pi.PropertyId);
            builder.HasIndex(pi => new { pi.PropertyId, pi.IsPrimary })
                .HasFilter("[IsPrimary] = 1");
            builder.HasIndex(pi => new { pi.PropertyId, pi.DisplayOrder });

            builder.HasOne(pi => pi.Property)
                .WithMany(p => p.PropertyImages)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
