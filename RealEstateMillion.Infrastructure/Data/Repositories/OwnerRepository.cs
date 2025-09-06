using Microsoft.EntityFrameworkCore;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Interfaces;
using RealEstateMillion.Infrastructure.Data.Context;

namespace RealEstateMillion.Infrastructure.Data.Repositories
{
    public class OwnerRepository(RealEstateMillionDbContext context) : Repository<Owner>(context), IOwnerRepository
    {
        public async Task<Owner?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(o => o.Properties)
                .FirstOrDefaultAsync(o => o.Email!.ToLower() == email.ToLower() && o.IsActive);
        }

        public async Task<Owner?> GetByDocumentAsync(string documentType, string documentNumber)
        {
            return await _dbSet
                .Include(o => o.Properties)
                .FirstOrDefaultAsync(o => o.DocumentType == documentType &&
                                        o.DocumentNumber == documentNumber &&
                                        o.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeOwnerId)
        {
            var query = _dbSet.Where(o => o.Email!.ToLower() == email.ToLower() && o.IsActive);

            if (excludeOwnerId.HasValue)
                query = query.Where(o => o.Id != excludeOwnerId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Owner>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(o => o.Name.ToLower().Contains(name.ToLower()) && o.IsActive)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }
    }

}
