using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;

namespace RealEstateMillion.Domain.Interfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<(IEnumerable<Property> Properties, int TotalCount)> GetPropertiesWithFiltersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            PropertyType? propertyType = null,
            PropertyStatus? status = null,
            ListingType? listingType = null,
            PropertyCondition? condition = null,
            int? minBedrooms = null,
            int? maxBedrooms = null,
            int? minBathrooms = null,
            int? maxBathrooms = null,
            decimal? minSquareFeet = null,
            decimal? maxSquareFeet = null,
            string? city = null,
            string? state = null,
            string? zipCode = null,
            string? neighborhood = null,
            bool? hasPool = null,
            bool? hasGarden = null,
            bool? hasGarage = null,
            bool? hasFireplace = null,
            bool? petsAllowed = null,
            int? minYear = null,
            int? maxYear = null,
            string sortBy = "CreatedAt",
            bool sortDescending = true);

        Task<Property?> GetByCodeInternalAsync(string codeInternal);
        Task<bool> CodeInternalExistsAsync(string codeInternal, Guid? excludePropertyId);
        Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<Property>> GetAvailablePropertiesAsync();
        Task<IEnumerable<Property>> GetPropertiesByLocationAsync(string city, string? state = null);
        Task<decimal> GetAveragePriceByTypeAsync(PropertyType propertyType);
    }
}
