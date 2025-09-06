using RealEstateMillion.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealEstateMillion.Application.DTOs.Property
{
    public class CreatePropertyRequest
    {
        [Required(ErrorMessage = "Property name is required")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Internal code is required")]
        [StringLength(20, ErrorMessage = "Internal code cannot exceed 20 characters")]
        public string CodeInternal { get; set; } = null!;

        [Required(ErrorMessage = "Year is required")]
        [Range(1800, 2030, ErrorMessage = "Year must be between 1800 and 2030")]
        public int Year { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Property type is required")]
        public PropertyType PropertyType { get; set; }

        public ListingType ListingType { get; set; } = ListingType.Sale;
        public PropertyCondition Condition { get; set; } = PropertyCondition.Good;

        [Range(0, 50, ErrorMessage = "Bedrooms must be between 0 and 50")]
        public int? Bedrooms { get; set; }

        [Range(0, 50, ErrorMessage = "Bathrooms must be between 0 and 50")]
        public int? Bathrooms { get; set; }

        [Range(0, 20, ErrorMessage = "Half bathrooms must be between 0 and 20")]
        public int? HalfBathrooms { get; set; }

        [Range(0, 20, ErrorMessage = "Parking spaces must be between 0 and 20")]
        public int? ParkingSpaces { get; set; }

        [Range(0.1, 1000000, ErrorMessage = "Square feet must be between 0.1 and 1,000,000")]
        public decimal? SquareFeet { get; set; }

        [Range(0.1, 10000000, ErrorMessage = "Lot size must be between 0.1 and 10,000,000")]
        public decimal? LotSize { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZIP code format")]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? Neighborhood { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        public bool HasPool { get; set; }
        public bool HasGarden { get; set; }
        public bool HasGarage { get; set; }
        public bool HasFireplace { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasHeating { get; set; }
        public bool IsFurnished { get; set; }
        public bool PetsAllowed { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Monthly rent must be greater than or equal to 0")]
        public decimal? MonthlyRent { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Property tax must be greater than or equal to 0")]
        public decimal? PropertyTax { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "HOA fees must be greater than or equal to 0")]
        public decimal? HOAFees { get; set; }

        public DateTime? AvailableFrom { get; set; }

        [Required(ErrorMessage = "Owner ID is required")]
        public Guid OwnerId { get; set; }
    }
}
