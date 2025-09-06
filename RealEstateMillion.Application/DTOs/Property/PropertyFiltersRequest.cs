using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Application.DTOs.Property
{
    public class PropertyFiltersRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public PropertyType? PropertyType { get; set; }
        public PropertyStatus? Status { get; set; }
        public ListingType? ListingType { get; set; }
        public PropertyCondition? Condition { get; set; }

        public int? MinBedrooms { get; set; }
        public int? MaxBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
        public int? MaxBathrooms { get; set; }

        public decimal? MinSquareFeet { get; set; }
        public decimal? MaxSquareFeet { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Neighborhood { get; set; }

        public bool? HasPool { get; set; }
        public bool? HasGarden { get; set; }
        public bool? HasGarage { get; set; }
        public bool? HasFireplace { get; set; }
        public bool? PetsAllowed { get; set; }

        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }

        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }
}
