using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Tests.TestHelpers
{
    public static class TestDataBuilder
    {
        public static CreatePropertyRequest CreateValidPropertyRequest()
        {
            return new CreatePropertyRequest
            {
                Name = "Test Property",
                Address = "123 Test Street",
                Price = 200000m,
                CodeInternal = "TEST001",
                Year = 2020,
                PropertyType = PropertyType.House,
                ListingType = ListingType.Sale,
                Bedrooms = 3,
                Bathrooms = 2,
                City = "Test City",
                State = "FL",
                ZipCode = "12345",
                OwnerId = Guid.NewGuid()
            };
        }

        public static Property CreateValidProperty(Guid id, Guid ownerId )
        {
            id = Guid.NewGuid();
            ownerId = Guid.NewGuid();
            return new Property
            {
                Id = id,
                Name = "Test Property",
                Address = "123 Test Street",
                Price = 200000m,
                CodeInternal = $"PROP{id:D3}",
                Year = 2020,
                PropertyType = PropertyType.House,
                Status = PropertyStatus.Available,
                ListingType = ListingType.Sale,
                Bedrooms = 3,
                Bathrooms = 2,
                City = "Test City",
                State = "FL",
                ZipCode = "12345",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PropertyImages = [],
                PropertyTraces = []
            };
        }

        public static Owner CreateValidOwner(Guid id)
        {
            id = Guid.NewGuid();
            return new Owner
            {
                Id = id,
                Name = "Test Owner",
                Email = $"owner{id}@example.com",
                Address = "123 Owner Street",
                City = "Test City",
                State = "FL",
                ZipCode = "12345",
                Phone = "555-0123",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static PropertyImage CreateValidPropertyImage(Guid id, Guid propertyId, bool isPrimary = false)
        {
            id = Guid.NewGuid();
            propertyId = Guid.NewGuid();
            return new PropertyImage
            {
                Id = id,
                PropertyId = propertyId,
                File = $"/uploads/test-image-{id}.jpg",
                Title = $"Test Image {id}",
                Description = $"Description for test image {id}",
                DisplayOrder = 1,
                IsPrimary = isPrimary,
                Enabled = true,
                FileType = "jpg",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static UpdatePropertyRequest CreateValidUpdateRequest()
        {
            return new UpdatePropertyRequest
            {
                Name = "Updated Property Name",
                Price = 250000m,
                Bedrooms = 4,
                Bathrooms = 3,
                City = "Updated City"
            };
        }

        public static ChangePriceRequest CreateValidChangePriceRequest(decimal newPrice = 275000m)
        {
            return new ChangePriceRequest
            {
                NewPrice = newPrice,
                Reason = "Market appreciation"
            };
        }
    }
}
