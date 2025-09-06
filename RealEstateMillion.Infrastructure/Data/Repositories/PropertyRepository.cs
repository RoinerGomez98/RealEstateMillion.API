using Microsoft.EntityFrameworkCore;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;

namespace RealEstateMillion.Infrastructure.Data.Repositories
{
    public class PropertyRepository(RealEstateMillionDbContext context) : Repository<Property>(context), IPropertyRepository
    {
        public async Task<(IEnumerable<Property> Properties, int TotalCount)> GetPropertiesWithFiltersAsync(
            int pageNumber, int pageSize, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null,
            PropertyType? propertyType = null, PropertyStatus? status = null, ListingType? listingType = null,
            PropertyCondition? condition = null, int? minBedrooms = null, int? maxBedrooms = null,
            int? minBathrooms = null, int? maxBathrooms = null, decimal? minSquareFeet = null, decimal? maxSquareFeet = null,
            string? city = null, string? state = null, string? zipCode = null, string? neighborhood = null,
            bool? hasPool = null, bool? hasGarden = null, bool? hasGarage = null, bool? hasFireplace = null,
            bool? petsAllowed = null, int? minYear = null, int? maxYear = null,
            string sortBy = "CreatedAt", bool sortDescending = true)
        {
            var query = _dbSet.Include(p => p.Owner).Include(p => p.PropertyImages).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Address.ToLower().Contains(term) ||
                    p.Description!.ToLower().Contains(term) ||
                    p.City!.ToLower().Contains(term) ||
                    p.Neighborhood!.ToLower().Contains(term) ||
                    p.CodeInternal.ToLower().Contains(term));
            }

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (propertyType.HasValue)
                query = query.Where(p => p.PropertyType == propertyType.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (listingType.HasValue)
                query = query.Where(p => p.ListingType == listingType.Value);

            if (condition.HasValue)
                query = query.Where(p => p.Condition == condition.Value);

            if (minBedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= minBedrooms.Value);

            if (maxBedrooms.HasValue)
                query = query.Where(p => p.Bedrooms <= maxBedrooms.Value);

            if (minBathrooms.HasValue)
                query = query.Where(p => p.Bathrooms >= minBathrooms.Value);

            if (maxBathrooms.HasValue)
                query = query.Where(p => p.Bathrooms <= maxBathrooms.Value);

            if (minSquareFeet.HasValue)
                query = query.Where(p => p.SquareFeet >= minSquareFeet.Value);

            if (maxSquareFeet.HasValue)
                query = query.Where(p => p.SquareFeet <= maxSquareFeet.Value);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => p.City!.ToLower().Contains(city.ToLower()));

            if (!string.IsNullOrWhiteSpace(state))
                query = query.Where(p => p.State!.ToLower().Contains(state.ToLower()));

            if (!string.IsNullOrWhiteSpace(zipCode))
                query = query.Where(p => p.ZipCode == zipCode);

            if (!string.IsNullOrWhiteSpace(neighborhood))
                query = query.Where(p => p.Neighborhood!.ToLower().Contains(neighborhood.ToLower()));

            if (hasPool.HasValue)
                query = query.Where(p => p.HasPool == hasPool.Value);

            if (hasGarden.HasValue)
                query = query.Where(p => p.HasGarden == hasGarden.Value);

            if (hasGarage.HasValue)
                query = query.Where(p => p.HasGarage == hasGarage.Value);

            if (hasFireplace.HasValue)
                query = query.Where(p => p.HasFireplace == hasFireplace.Value);

            if (petsAllowed.HasValue)
                query = query.Where(p => p.PetsAllowed == petsAllowed.Value);

            if (minYear.HasValue)
                query = query.Where(p => p.Year >= minYear.Value);

            if (maxYear.HasValue)
                query = query.Where(p => p.Year <= maxYear.Value);

            query = query.Where(p => p.IsActive);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, sortBy, sortDescending);

            var properties = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (properties, totalCount);
        }

        private static IQueryable<Property> ApplySorting(IQueryable<Property> query, string sortBy, bool sortDescending)
        {
            return sortBy.ToLower() switch
            {
                "name" => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => sortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "year" => sortDescending ? query.OrderByDescending(p => p.Year) : query.OrderBy(p => p.Year),
                "createdat" => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "updatedat" => sortDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
                "city" => sortDescending ? query.OrderByDescending(p => p.City) : query.OrderBy(p => p.City),
                "bedrooms" => sortDescending ? query.OrderByDescending(p => p.Bedrooms) : query.OrderBy(p => p.Bedrooms),
                "bathrooms" => sortDescending ? query.OrderByDescending(p => p.Bathrooms) : query.OrderBy(p => p.Bathrooms),
                "squarefeet" => sortDescending ? query.OrderByDescending(p => p.SquareFeet) : query.OrderBy(p => p.SquareFeet),
                _ => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
            };
        }

        public async Task<Property?> GetByCodeInternalAsync(string codeInternal)
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .Include(p => p.PropertyTraces)
                .FirstOrDefaultAsync(p => p.CodeInternal == codeInternal && p.IsActive);
        }

        public async Task<bool> CodeInternalExistsAsync(string codeInternal, Guid? excludePropertyId = null)
        {
            var query = _dbSet.Where(p => p.CodeInternal == codeInternal && p.IsActive);

            if (excludePropertyId.HasValue)
                query = query.Where(p => p.Id != excludePropertyId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _dbSet
                .Include(p => p.PropertyImages)
                .Where(p => p.OwnerId == ownerId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync()
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .Where(p => p.Status == PropertyStatus.Available && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLocationAsync(string city, string? state = null)
        {
            var query = _dbSet
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .Where(p => p.City!.ToLower().Contains(city.ToLower()) && p.IsActive);

            if (!string.IsNullOrWhiteSpace(state))
                query = query.Where(p => p.State!.ToLower().Contains(state.ToLower()));

            return await query.ToListAsync();
        }

        public async Task<decimal> GetAveragePriceByTypeAsync(PropertyType propertyType)
        {
            return await _dbSet
                .Where(p => p.PropertyType == propertyType && p.IsActive && p.Status == PropertyStatus.Available)
                .AverageAsync(p => p.Price);
        }
    }
}
