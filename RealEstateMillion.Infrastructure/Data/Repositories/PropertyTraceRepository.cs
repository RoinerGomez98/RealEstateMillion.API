using Microsoft.EntityFrameworkCore;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;

namespace RealEstateMillion.Infrastructure.Data.Repositories
{
    public class PropertyTraceRepository(RealEstateMillionDbContext context) : Repository<PropertyTrace>(context), IPropertyTraceRepository
    {
        public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _dbSet
                .Where(pt => pt.PropertyId == propertyId && pt.IsActive)
                .OrderByDescending(pt => pt.DateSale)
                .ThenByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }

        public async Task<PropertyTrace?> GetLastTraceAsync(Guid propertyId)
        {
            return await _dbSet
                .Where(pt => pt.PropertyId == propertyId && pt.IsActive)
                .OrderByDescending(pt => pt.DateSale)
                .ThenByDescending(pt => pt.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PropertyTrace>> GetTracesByDateRangeAsync(Guid propertyId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(pt => pt.PropertyId == propertyId && pt.IsActive &&
                            pt.DateSale >= startDate && pt.DateSale <= endDate)
                .OrderByDescending(pt => pt.DateSale)
                .ToListAsync();
        }
    }

}
