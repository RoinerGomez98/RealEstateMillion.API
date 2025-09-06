using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;
using RealEstateMillion.Infrastructure.Data.Context;
using RealEstateMillion.Infrastructure.Data.Repositories;

namespace RealEstateMillion.Tests.Repositories
{
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private RealEstateMillionDbContext _context;
        private PropertyRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RealEstateMillionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RealEstateMillionDbContext(options);
            _repository = new PropertyRepository(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var n1 = Guid.NewGuid();
            var n2 = Guid.NewGuid();

            var owners = new List<Owner>
        {
            new() {
                Id = n1,
                Name = "John Smith",
                Email = "john@example.com",
                Address = "123 Main St",
                City = "Miami",
                State = "FL"
            },
            new() {
                Id = n2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                Address = "456 Oak Ave",
                City = "Orlando",
                State = "FL"
            }
        };

            var properties = new List<Property>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Beautiful House",
                Address = "123 Property St, Miami, FL",
                Price = 250000m,
                CodeInternal = "PROP001",
                Year = 2020,
                PropertyType = PropertyType.House,
                Status = PropertyStatus.Available,
                ListingType = ListingType.Sale,
                Bedrooms = 3,
                Bathrooms = 2,
                City = "Miami",
                State = "FL",
                ZipCode = "33101",
                HasPool = true,
                HasGarage = true,
                OwnerId = n1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Modern Apartment",
                Address = "456 Apt Blvd, Orlando, FL",
                Price = 180000m,
                CodeInternal = "PROP002",
                Year = 2019,
                PropertyType = PropertyType.Apartment,
                Status = PropertyStatus.Available,
                ListingType = ListingType.Sale,
                Bedrooms = 2,
                Bathrooms = 1,
                City = "Orlando",
                State = "FL",
                ZipCode = "32801",
                HasPool = false,
                HasGarage = false,
                OwnerId = n2,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Luxury Villa",
                Address = "789 Villa Dr, Miami, FL",
                Price = 500000m,
                CodeInternal = "PROP003",
                Year = 2021,
                PropertyType = PropertyType.Villa,
                Status = PropertyStatus.Sold,
                ListingType = ListingType.Sale,
                Bedrooms = 5,
                Bathrooms = 4,
                City = "Miami",
                State = "FL",
                ZipCode = "33102",
                HasPool = true,
                HasGarage = true,
                OwnerId = n1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

            _context.Owners.AddRange(owners);
            _context.Property.AddRange(properties);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithCityFilter_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                city: "Miami");

   
            properties.Should().HaveCount(2);
            totalCount.Should().Be(2);
            properties.Should().OnlyContain(p => p.City == "Miami");
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithPriceRange_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                minPrice: 200000m,
                maxPrice: 300000m);

   
            properties.Should().HaveCount(1);
            totalCount.Should().Be(1);
            properties.First().Price.Should().Be(250000m);
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithPropertyType_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                propertyType: PropertyType.House);

   
            properties.Should().HaveCount(1);
            totalCount.Should().Be(1);
            properties.First().PropertyType.Should().Be(PropertyType.House);
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithBedroomFilter_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                minBedrooms: 3);

   
            properties.Should().HaveCount(2);
            totalCount.Should().Be(2);
            properties.Should().OnlyContain(p => p.Bedrooms >= 3);
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithFeatureFilter_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                hasPool: true);

   
            properties.Should().HaveCount(2);
            totalCount.Should().Be(2);
            properties.Should().OnlyContain(p => p.HasPool == true);
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithSearchTerm_ShouldReturnFilteredResults()
        {

            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 1,
                pageSize: 10,
                searchTerm: "Modern");

   
            properties.Should().HaveCount(1);
            totalCount.Should().Be(1);
            properties.First().Name.Should().Contain("Modern");
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithPagination_ShouldReturnCorrectPage()
        {
            var (properties, totalCount) = await _repository.GetPropertiesWithFiltersAsync(
                pageNumber: 2,
                pageSize: 2);


            properties.Should().HaveCount(1);
            totalCount.Should().Be(3);
        }

        [Test]
        public async Task GetByCodeInternalAsync_WithExistingCode_ShouldReturnProperty()
        {
            var property = await _repository.GetByCodeInternalAsync("PROP001");

            property.Should().NotBeNull();
            property!.CodeInternal.Should().Be("PROP001");
            property.Name.Should().Be("Beautiful House");
            property.Owner.Should().NotBeNull();
            property.Owner.Name.Should().Be("John Smith");
        }

        [Test]
        public async Task GetByCodeInternalAsync_WithNonExistentCode_ShouldReturnNull()
        {

            var property = await _repository.GetByCodeInternalAsync("NONEXISTENT");

   
            property.Should().BeNull();
        }

        [Test]
        public async Task CodeInternalExistsAsync_WithExistingCode_ShouldReturnTrue()
        {

            var exists = await _repository.CodeInternalExistsAsync("PROP001");

   
            exists.Should().BeTrue();
        }

        [Test]
        public async Task CodeInternalExistsAsync_WithNonExistentCode_ShouldReturnFalse()
        {

            var exists = await _repository.CodeInternalExistsAsync("NONEXISTENT");

   
            exists.Should().BeFalse();
        }

        [Test]
        public async Task CodeInternalExistsAsync_WithExcludedId_ShouldReturnCorrectResult()
        {
            var ownId = Guid.NewGuid();
            var exists = await _repository.CodeInternalExistsAsync("PROP001", excludePropertyId: ownId);

   
            exists.Should().BeFalse(); 
        }

        [Test]
        public async Task GetByOwnerIdAsync_WithExistingOwner_ShouldReturnProperties()
        {

            var ownId = Guid.NewGuid();
            var properties = await _repository.GetByOwnerIdAsync(ownId);

   
            properties.Should().HaveCount(2);
            properties.Should().OnlyContain(p => p.OwnerId == ownId);
            properties.Should().Contain(p => p.CodeInternal == "PROP001");
            properties.Should().Contain(p => p.CodeInternal == "PROP003");
        }

        [Test]
        public async Task GetAvailablePropertiesAsync_ShouldReturnOnlyAvailableProperties()
        {

            var properties = await _repository.GetAvailablePropertiesAsync();

   
            properties.Should().HaveCount(2);
            properties.Should().OnlyContain(p => p.Status == PropertyStatus.Available);
            properties.Should().NotContain(p => p.Status == PropertyStatus.Sold);
        }

        [Test]
        public async Task GetPropertiesByLocationAsync_WithCityOnly_ShouldReturnFilteredResults()
        {

            var properties = await _repository.GetPropertiesByLocationAsync("Miami");

   
            properties.Should().HaveCount(2);
            properties.Should().OnlyContain(p => p.City == "Miami");
        }

        [Test]
        public async Task GetPropertiesByLocationAsync_WithCityAndState_ShouldReturnFilteredResults()
        {

            var properties = await _repository.GetPropertiesByLocationAsync("Orlando", "FL");

   
            properties.Should().HaveCount(1);
            properties.First().City.Should().Be("Orlando");
            properties.First().State.Should().Be("FL");
        }

        [Test]
        public async Task GetAveragePriceByTypeAsync_WithValidType_ShouldReturnCorrectAverage()
        {

            var averagePrice = await _repository.GetAveragePriceByTypeAsync(PropertyType.House);

   
            averagePrice.Should().Be(250000m);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

}
