using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Domain.Interfaces
{
    public interface IPropertyImageRepository : IRepository<PropertyImage>
    {
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);
        Task<PropertyImage?> GetPrimaryImageAsync(Guid propertyId);
        Task<int> GetMaxDisplayOrderAsync(Guid propertyId);
        Task DisablePrimaryImagesAsync(Guid propertyId);
    }
}
