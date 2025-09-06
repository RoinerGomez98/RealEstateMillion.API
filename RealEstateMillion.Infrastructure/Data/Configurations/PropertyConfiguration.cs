using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Infrastructure.Data.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Property");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CodeInternal)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Year)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.PropertyType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(PropertyStatus.Available);

            builder.Property(p => p.ListingType)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ListingType.Sale);

            builder.Property(p => p.Condition)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(PropertyCondition.Good);

            builder.Property(p => p.SquareFeet)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.LotSize)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.City)
                .HasMaxLength(100);

            builder.Property(p => p.State)
                .HasMaxLength(50);

            builder.Property(p => p.ZipCode)
                .HasMaxLength(10);

            builder.Property(p => p.Neighborhood)
                .HasMaxLength(100);

            builder.Property(p => p.Latitude)
                .HasColumnType("decimal(10,8)");

            builder.Property(p => p.Longitude)
                .HasColumnType("decimal(11,8)");

            builder.Property(p => p.MonthlyRent)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PropertyTax)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.HOAFees)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.CreatedBy)
                .HasMaxLength(100);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(p => p.CodeInternal)
                .IsUnique();

            builder.HasIndex(p => p.PropertyType);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.ListingType);
            builder.HasIndex(p => p.Price);
            builder.HasIndex(p => new { p.City, p.State });
            builder.HasIndex(p => p.ZipCode);
            builder.HasIndex(p => p.CreatedAt);

            builder.HasOne(p => p.Owner)
                .WithMany(o => o.Properties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.PropertyImages)
                .WithOne(pi => pi.Property)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PropertyTraces)
                .WithOne(pt => pt.Property)
                .HasForeignKey(pt => pt.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
