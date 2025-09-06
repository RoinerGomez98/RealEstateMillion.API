using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Application.DTOs.Property
{
    public class PropertyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = null!;
        public int Year { get; set; }
        public string? Description { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyTypeName { get; set; } = null!;
        public PropertyStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public ListingType ListingType { get; set; }
        public string ListingTypeName { get; set; } = null!;
        public PropertyCondition Condition { get; set; }
        public string ConditionName { get; set; } = null!;

        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? HalfBathrooms { get; set; }
        public int? ParkingSpaces { get; set; }
        public decimal? SquareFeet { get; set; }
        public decimal? LotSize { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Neighborhood { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool HasPool { get; set; }
        public bool HasGarden { get; set; }
        public bool HasGarage { get; set; }
        public bool HasFireplace { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasHeating { get; set; }
        public bool IsFurnished { get; set; }
        public bool PetsAllowed { get; set; }

        public decimal? MonthlyRent { get; set; }
        public decimal? PropertyTax { get; set; }
        public decimal? HOAFees { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? ListedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = null!;

        public List<PropertyImageResponse> Images { get; set; } = [];
        public string? PrimaryImageUrl { get; set; }
    }
}
