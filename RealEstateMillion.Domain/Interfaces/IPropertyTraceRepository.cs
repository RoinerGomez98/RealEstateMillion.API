using RealEstateMillion.Domain.Entities;

namespace RealEstateMillion.Domain.Interfaces
{
    public interface IPropertyTraceRepository : IRepository<PropertyTrace>
    {
        Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId);
        Task<PropertyTrace?> GetLastTraceAsync(Guid propertyId);
        Task<IEnumerable<PropertyTrace>> GetTracesByDateRangeAsync(Guid propertyId, DateTime startDate, DateTime endDate);
    }
}
