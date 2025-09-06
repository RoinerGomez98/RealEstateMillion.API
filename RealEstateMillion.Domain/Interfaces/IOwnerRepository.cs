using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Domain.Interfaces
{
    public interface IOwnerRepository : IRepository<Owner>
    {
        Task<Owner?> GetByEmailAsync(string email);
        Task<Owner?> GetByDocumentAsync(string documentType, string documentNumber);
        Task<bool> EmailExistsAsync(string email, Guid? excludeOwnerId);
        Task<IEnumerable<Owner>> SearchByNameAsync(string name);
    }
}
