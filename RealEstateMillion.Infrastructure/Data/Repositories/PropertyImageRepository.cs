using Microsoft.EntityFrameworkCore;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;

namespace RealEstateMillion.Infrastructure.Data.Repositories
{
    public class PropertyImageRepository(RealEstateMillionDbContext context) : Repository<PropertyImage>(context), IPropertyImageRepository
    {
        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _dbSet
                .Where(pi => pi.PropertyId == propertyId && pi.Enabled && pi.IsActive)
                .OrderBy(pi => pi.DisplayOrder)
                .ThenBy(pi => pi.CreatedAt)
                .ToListAsync();
        }

        public async Task<PropertyImage?> GetPrimaryImageAsync(Guid propertyId)
        {
            return await _dbSet
                .Where(pi => pi.PropertyId == propertyId && pi.IsPrimary && pi.Enabled && pi.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetMaxDisplayOrderAsync(Guid propertyId)
        {
            var maxOrder = await _dbSet
                .Where(pi => pi.PropertyId == propertyId && pi.IsActive)
                .MaxAsync(pi => (int?)pi.DisplayOrder);

            return maxOrder ?? 0;
        }

        public async Task DisablePrimaryImagesAsync(Guid propertyId)
        {
            var primaryImages = await _dbSet
                .Where(pi => pi.PropertyId == propertyId && pi.IsPrimary && pi.IsActive)
                .ToListAsync();

            foreach (var image in primaryImages)
            {
                image.IsPrimary = false;
                image.UpdatedAt = DateTime.UtcNow;
            }

            _dbSet.UpdateRange(primaryImages);
        }
    }
}
