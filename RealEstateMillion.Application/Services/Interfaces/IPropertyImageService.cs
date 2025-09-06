using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.PropertyImage;

namespace RealEstateMillion.Application.Services.Interfaces
{
    public interface IPropertyImageService
    {
        Task<ApiResponse<PropertyImageResponse>> AddImageAsync(AddImageRequest request);
        Task<ApiResponse<IEnumerable<PropertyImageResponse>>> GetImagesByPropertyAsync(Guid propertyId);
        Task<ApiResponse<PropertyImageResponse>> SetPrimaryImageAsync(Guid imageId);
        Task<ApiResponse<bool>> DeleteImageAsync(Guid imageId);
        Task<ApiResponse<PropertyImageResponse>> UpdateImageAsync(Guid imageId, AddImageRequest request);
    }
}
