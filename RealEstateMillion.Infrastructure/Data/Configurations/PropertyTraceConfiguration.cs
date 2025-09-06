using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Infrastructure.Data.Configurations
{
    public class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
    {
        public void Configure(EntityTypeBuilder<PropertyTrace> builder)
        {
            builder.ToTable("PropertyTraces");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.Value)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(pt => pt.Tax)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(pt => pt.Description)
                .HasMaxLength(500);

            builder.Property(pt => pt.TransactionType)
                .HasMaxLength(100);

            builder.Property(pt => pt.AgentName)
                .HasMaxLength(100);

            builder.Property(pt => pt.BuyerName)
                .HasMaxLength(100);

            builder.Property(pt => pt.SellerName)
                .HasMaxLength(100);

            builder.Property(pt => pt.CommissionRate)
                .HasColumnType("decimal(5,2)");

            builder.Property(pt => pt.CommissionAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(pt => pt.CreatedBy)
                .HasMaxLength(100);

            builder.Property(pt => pt.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(pt => pt.PropertyId);
            builder.HasIndex(pt => pt.DateSale);
            builder.HasIndex(pt => pt.TransactionType);

            builder.HasOne(pt => pt.Property)
                .WithMany(p => p.PropertyTraces)
                .HasForeignKey(pt => pt.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
