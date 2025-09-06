using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Property;

namespace RealEstateMillion.Application.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<ApiResponse<PropertyResponse>> CreatePropertyAsync(CreatePropertyRequest request);
        Task<ApiResponse<PropertyResponse>> UpdatePropertyAsync(Guid propertyId, UpdatePropertyRequest request);
        Task<ApiResponse<PropertyResponse>> ChangePriceAsync(Guid propertyId, ChangePriceRequest request);
        Task<ApiResponse<PropertyResponse>> GetPropertyByIdAsync(Guid propertyId);
        Task<ApiResponse<PropertyResponse>> GetPropertyByCodeAsync(string codeInternal);
        Task<ApiResponse<PagedResult<PropertyResponse>>> GetPropertiesWithFiltersAsync(PropertyFiltersRequest filters);
        Task<ApiResponse<bool>> DeletePropertyAsync(Guid propertyId);
        Task<ApiResponse<IEnumerable<PropertyResponse>>> GetPropertiesByOwnerAsync(Guid ownerId);
    }
}
