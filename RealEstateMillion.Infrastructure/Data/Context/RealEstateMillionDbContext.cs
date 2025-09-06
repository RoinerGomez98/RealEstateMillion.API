using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealEstateMillion.Domain.Common;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Infrastructure.Data.Configurations;

namespace RealEstateMillion.Infrastructure.Data.Context
{
    public class RealEstateMillionDbContext(DbContextOptions<RealEstateMillionDbContext> options) : DbContext(options)
    {
        public DbSet<Owner> Owners { get; set; } = null!;
        public DbSet<Property> Property { get; set; } = null!;
        public DbSet<PropertyImage> PropertyImages { get; set; } = null!;
        public DbSet<PropertyTrace> PropertyTraces { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OwnerConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyImageConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyTraceConfiguration());

            modelBuilder.Entity<Owner>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Property>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<PropertyImage>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<PropertyTrace>().HasQueryFilter(e => e.IsActive);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Owner>().HasData(
                new Owner
                {
                    Id = Guid.NewGuid(),
                    Name = "John Smith",
                    Address = "123 Main St, New York, NY 10001",
                    Phone = "+1-555-0123",
                    Email = "john.smith@email.com",
                    DocumentType = "SSN",
                    DocumentNumber = "123-45-6789",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Owner
                {
                    Id = Guid.NewGuid(),
                    Name = "Maria Garcia",
                    Address = "456 Oak Ave, Miami, FL 33101",
                    Phone = "+1-555-0456",
                    Email = "maria.garcia@email.com",
                    DocumentType = "SSN",
                    DocumentNumber = "987-65-4321",
                    City = "Miami",
                    State = "FL",
                    ZipCode = "33101",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                 new Owner
                 {
                     Id = Guid.NewGuid(),
                     Name = "ROINER GOMEZ",
                     Address = "CRA 1324 AV 12 #4 ",
                     Phone = "3209055901",
                     Email = "rstiven_98@hotmail.com",
                     DocumentType = "CC",
                     DocumentNumber = "11-22-334455",
                     City = "Soacha",
                     State = "FL",
                     ZipCode = "33101",
                     Country = "CO",
                     CreatedAt = DateTime.UtcNow,
                     IsActive = true
                 }
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsActive = true;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsActive = false;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
