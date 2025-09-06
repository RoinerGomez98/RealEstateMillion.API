using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstateMillion.Domain.Common;
using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Domain.Entities
{
    public class Property : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string Address { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string CodeInternal { get; set; } = null!;

        public int Year { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public PropertyType PropertyType { get; set; }

        public PropertyStatus Status { get; set; } = PropertyStatus.Available;

        public ListingType ListingType { get; set; } = ListingType.Sale;

        public PropertyCondition Condition { get; set; } = PropertyCondition.Good;
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? HalfBathrooms { get; set; }
        public int? ParkingSpaces { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SquareFeet { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? LotSize { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(100)]
        public string? Neighborhood { get; set; }

        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        public bool HasPool { get; set; }
        public bool HasGarden { get; set; }
        public bool HasGarage { get; set; }
        public bool HasFireplace { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasHeating { get; set; }
        public bool IsFurnished { get; set; }
        public bool PetsAllowed { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PropertyTax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? HOAFees { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? ListedDate { get; set; }
        public DateTime? SoldDate { get; set; }

        public Guid OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual Owner Owner { get; set; } = null!;

        public virtual ICollection<PropertyImage> PropertyImages { get; set; } = [];

        public virtual ICollection<PropertyTrace> PropertyTraces { get; set; } = [];
    }
}
