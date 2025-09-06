using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Application.Services.Interfaces;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Application.Services.Implementations
{
    public class PropertyImageService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PropertyImageService> logger) : IPropertyImageService
    {
        public async Task<ApiResponse<PropertyImageResponse>> AddImageAsync(AddImageRequest request)
        {
            try
            {
                logger.LogInformation("Adding image to property: {PropertyId}", request.PropertyId);

                var property = await unitOfWork.Properties.GetByIdAsync(request.PropertyId);
                if (property == null)
                {
                    logger.LogWarning("Property not found: {PropertyId}", request.PropertyId);
                    return ApiResponse<PropertyImageResponse>.ErrorResponse("Property not found", 404);
                }

                if (request.IsPrimary)
                {
                    await unitOfWork.PropertyImages.DisablePrimaryImagesAsync(request.PropertyId);
                }

                if (request.DisplayOrder == 0)
                {
                    var maxOrder = await unitOfWork.PropertyImages.GetMaxDisplayOrderAsync(request.PropertyId);
                    request.DisplayOrder = maxOrder + 1;
                }

                
                var propertyImage = mapper.Map<PropertyImage>(request);

                propertyImage.FileType = ExtractFileType(request.File);
                propertyImage.ThumbnailPath = GenerateThumbnailPath(request.File);

                await unitOfWork.PropertyImages.AddAsync(propertyImage);
                await unitOfWork.SaveChangesAsync();

                var response = mapper.Map<PropertyImageResponse>(propertyImage);

                logger.LogInformation("Image added successfully: {ImageId}", propertyImage.Id);
                return ApiResponse<PropertyImageResponse>.SuccessResponse(response, "Image added successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding image to property: {PropertyId}", request.PropertyId);
                return ApiResponse<PropertyImageResponse>.ErrorResponse("An error occurred while adding the image", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<PropertyImageResponse>>> GetImagesByPropertyAsync(Guid propertyId)
        {
            try
            {
                var images = await unitOfWork.PropertyImages.GetByPropertyIdAsync(propertyId);
                var response = mapper.Map<IEnumerable<PropertyImageResponse>>(images);

                return ApiResponse<IEnumerable<PropertyImageResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting images for property: {PropertyId}", propertyId);
                return ApiResponse<IEnumerable<PropertyImageResponse>>.ErrorResponse(
                    "An error occurred while retrieving images", 500);
            }
        }

        public async Task<ApiResponse<PropertyImageResponse>> SetPrimaryImageAsync(Guid imageId)
        {
            try
            {
                logger.LogInformation("Setting primary image: {ImageId}", imageId);

                var image = await unitOfWork.PropertyImages.GetByIdAsync(imageId);
                if (image == null)
                {
                    logger.LogWarning("Image not found: {ImageId}", imageId);
                    return ApiResponse<PropertyImageResponse>.ErrorResponse("Image not found", 404);
                }


                await unitOfWork.PropertyImages.DisablePrimaryImagesAsync(image.PropertyId);


                image.IsPrimary = true;
                image.UpdatedAt = DateTime.UtcNow;

                unitOfWork.PropertyImages.Update(image);
                await unitOfWork.SaveChangesAsync();

                var response = mapper.Map<PropertyImageResponse>(image);

                logger.LogInformation("Primary image set successfully: {ImageId}", imageId);
                return ApiResponse<PropertyImageResponse>.SuccessResponse(response, "Primary image set successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error setting primary image: {ImageId}", imageId);
                return ApiResponse<PropertyImageResponse>.ErrorResponse("An error occurred while setting primary image", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(Guid imageId)
        {
            try
            {
                logger.LogInformation("Deleting image: {ImageId}", imageId);

                var image = await unitOfWork.PropertyImages.GetByIdAsync(imageId);
                if (image == null)
                {
                    logger.LogWarning("Image not found: {ImageId}", imageId);
                    return ApiResponse<bool>.ErrorResponse("Image not found", 404);
                }

                unitOfWork.PropertyImages.Remove(image);
                await unitOfWork.SaveChangesAsync();


                if (image.IsPrimary)
                {
                    var remainingImages = await unitOfWork.PropertyImages.GetByPropertyIdAsync(image.PropertyId);
                    var firstImage = remainingImages.FirstOrDefault();
                    if (firstImage != null)
                    {
                        firstImage.IsPrimary = true;
                        firstImage.UpdatedAt = DateTime.UtcNow;
                        unitOfWork.PropertyImages.Update(firstImage);
                        await unitOfWork.SaveChangesAsync();
                    }
                }

                logger.LogInformation("Image deleted successfully: {ImageId}", imageId);
                return ApiResponse<bool>.SuccessResponse(true, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting image: {ImageId}", imageId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the image", 500);
            }
        }

        public async Task<ApiResponse<PropertyImageResponse>> UpdateImageAsync(Guid imageId, AddImageRequest request)
        {
            try
            {
                logger.LogInformation("Updating image: {ImageId}", imageId);

                var image = await unitOfWork.PropertyImages.GetByIdAsync(imageId);
                if (image == null)
                {
                    logger.LogWarning("Image not found: {ImageId}", imageId);
                    return ApiResponse<PropertyImageResponse>.ErrorResponse("Image not found", 404);
                }

                if (request.IsPrimary && !image.IsPrimary)
                {
                    await unitOfWork.PropertyImages.DisablePrimaryImagesAsync(image.PropertyId);
                }

                image.File = request.File;
                image.Title = request.Title;
                image.Description = request.Description;
                image.DisplayOrder = request.DisplayOrder;
                image.IsPrimary = request.IsPrimary;
                image.FileType = ExtractFileType(request.File);
                image.ThumbnailPath = GenerateThumbnailPath(request.File);
                image.UpdatedAt = DateTime.UtcNow;

                unitOfWork.PropertyImages.Update(image);
                await unitOfWork.SaveChangesAsync();

                var response = mapper.Map<PropertyImageResponse>(image);

                logger.LogInformation("Image updated successfully: {ImageId}", imageId);
                return ApiResponse<PropertyImageResponse>.SuccessResponse(response, "Image updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating image: {ImageId}", imageId);
                return ApiResponse<PropertyImageResponse>.ErrorResponse("An error occurred while updating the image", 500);
            }
        }

        private static string? ExtractFileType(string filePath)
        {
            try
            {
                return Path.GetExtension(filePath)?.TrimStart('.').ToLower();
            }
            catch
            {
                return null;
            }
        }

        private static string? GenerateThumbnailPath(string originalPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(originalPath);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
                var extension = Path.GetExtension(originalPath);

                return Path.Combine(directory ?? "", $"{fileNameWithoutExtension}_thumb{extension}");
            }
            catch
            {
                return null;
            }
        }
    }
}
