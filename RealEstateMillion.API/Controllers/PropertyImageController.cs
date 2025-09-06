using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Application.Services.Interfaces;

namespace RealEstateMillion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/properties")]
    [Produces("application/json")]
    public class PropertyImagesController(IPropertyImageService propertyImageService, ILogger<PropertyImagesController> logger) : ControllerBase
    {

        /// <summary>
        /// Add image to property (REQUIRED ENDPOINT)
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="request">Image details</param>
        /// <returns>Added image information</returns>
        /// <response code="200">Image added successfully</response>
        /// <response code="400">Invalid image data</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{propertyId:Guid}/images")]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyImageResponse>>> AddImageToProperty(
            [FromRoute] Guid propertyId,
            [FromBody] AddImageToPropertyRequest request)
        {
            logger.LogInformation("Adding image to property: {PropertyId}", propertyId);

   
            var addImageRequest = new AddImageRequest
            {
                PropertyId = propertyId,
                File = request.File,
                Title = request.Title,
                Description = request.Description,
                DisplayOrder = request.DisplayOrder,
                IsPrimary = request.IsPrimary
            };

            var result = await propertyImageService.AddImageAsync(addImageRequest);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Add image to property using form data (alternative endpoint for file uploads)
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="file">Image file</param>
        /// <param name="title">Image title (optional)</param>
        /// <param name="description">Image description (optional)</param>
        /// <param name="displayOrder">Display order (optional, default: auto)</param>
        /// <param name="isPrimary">Set as primary image (optional, default: false)</param>
        /// <returns>Added image information</returns>
        /// <response code="200">Image uploaded successfully</response>
        /// <response code="400">Invalid file or data</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{propertyId:Guid}/images/upload")]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyImageResponse>>> UploadImageToProperty(
            [FromRoute] Guid propertyId, [FromForm] UploadImageRequest request)
        {
            try
            {
                logger.LogInformation("Uploading image file to property: {PropertyId}", propertyId);

                if (request.file == null || request.file.Length == 0)
                {
                    var badFileResponse = ApiResponse<PropertyImageResponse>.ErrorResponse("No file provided", 400);
                    return BadRequest(badFileResponse);
                }

                const long maxFileSize = 10 * 1024 * 1024;
                if (request.file.Length > maxFileSize)
                {
                    var fileSizeResponse = ApiResponse<PropertyImageResponse>.ErrorResponse("File size exceeds 10MB limit", 400);
                    return BadRequest(fileSizeResponse);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                var fileExtension = Path.GetExtension(request.file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    var extensionResponse = ApiResponse<PropertyImageResponse>.ErrorResponse(
                        $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}", 400);
                    return BadRequest(extensionResponse);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsPath = Path.Combine("uploads", "properties", propertyId.ToString());
                var fullUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadsPath);

                Directory.CreateDirectory(fullUploadsPath);

                var filePath = Path.Combine(fullUploadsPath, uniqueFileName);
                var relativeFilePath = Path.Combine(uploadsPath, uniqueFileName).Replace("\\", "/");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(stream);
                }

                var addImageRequest = new AddImageRequest
                {
                    PropertyId = propertyId,
                    File = $"/{relativeFilePath}",
                    Title = request.title,
                    Description = request.description,
                    DisplayOrder = request.displayOrder,
                    IsPrimary = request.isPrimary
                };

                var result = await propertyImageService.AddImageAsync(addImageRequest);

                if (!result.Success && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading image to property: {PropertyId}", propertyId);
                var errorResponse = ApiResponse<PropertyImageResponse>.ErrorResponse("An error occurred while uploading the image", 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Get all images for a property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>List of property images</returns>
        /// <response code="200">Images retrieved successfully</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{propertyId:Guid}/images")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyImageResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyImageResponse>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyImageResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyImageResponse>>>> GetPropertyImages(
            [FromRoute] Guid propertyId)
        {
            logger.LogInformation("Getting images for property: {PropertyId}", propertyId);

            var result = await propertyImageService.GetImagesByPropertyAsync(propertyId);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update property image
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="imageId">Image ID</param>
        /// <param name="request">Updated image details</param>
        /// <returns>Updated image information</returns>
        /// <response code="200">Image updated successfully</response>
        /// <response code="400">Invalid image data</response>
        /// <response code="404">Image not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{propertyId:Guid}/images/{imageId:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyImageResponse>>> UpdatePropertyImage(
            [FromRoute] Guid propertyId,
            [FromRoute] Guid imageId,
            [FromBody] AddImageToPropertyRequest request)
        {
            logger.LogInformation("Updating image: {ImageId} for property: {PropertyId}", imageId, propertyId);

            var updateRequest = new AddImageRequest
            {
                PropertyId = propertyId,
                File = request.File,
                Title = request.Title,
                Description = request.Description,
                DisplayOrder = request.DisplayOrder,
                IsPrimary = request.IsPrimary
            };

            var result = await propertyImageService.UpdateImageAsync(imageId, updateRequest);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Set image as primary for property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="imageId">Image ID to set as primary</param>
        /// <returns>Updated image information</returns>
        /// <response code="200">Primary image set successfully</response>
        /// <response code="404">Image not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{propertyId:Guid}/images/{imageId:Guid}/primary")]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyImageResponse>>> SetPrimaryImage(
            [FromRoute] Guid propertyId,
            [FromRoute] Guid imageId)
        {
            logger.LogInformation("Setting primary image: {ImageId} for property: {PropertyId}", imageId, propertyId);

            var result = await propertyImageService.SetPrimaryImageAsync(imageId);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete property image
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="imageId">Image ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Image deleted successfully</response>
        /// <response code="404">Image not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{propertyId:Guid}/images/{imageId:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePropertyImage(
            [FromRoute] Guid propertyId,
            [FromRoute] Guid imageId)
        {
            logger.LogInformation("Deleting image: {ImageId} for property: {PropertyId}", imageId, propertyId);

            var result = await propertyImageService.DeleteImageAsync(imageId);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get image by ID with property validation
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="imageId">Image ID</param>
        /// <returns>Image information</returns>
        /// <response code="200">Image found</response>
        /// <response code="404">Image not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{propertyId:Guid}/images/{imageId:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyImageResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyImageResponse>>> GetPropertyImage(
            [FromRoute] Guid propertyId,
            [FromRoute] Guid imageId)
        {
            try
            {
                logger.LogInformation("Getting image: {ImageId} for property: {PropertyId}", imageId, propertyId);

                var imagesResult = await propertyImageService.GetImagesByPropertyAsync(propertyId);

                if (!imagesResult.Success)
                {
                    return StatusCode(imagesResult.StatusCode,
                        ApiResponse<PropertyImageResponse>.ErrorResponse(imagesResult.Message, imagesResult.StatusCode));
                }

                var image = imagesResult.Data?.FirstOrDefault(img => img.Id == imageId);

                if (image == null)
                {
                    var notFoundResponse = ApiResponse<PropertyImageResponse>.ErrorResponse("Image not found", 404);
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<PropertyImageResponse>.SuccessResponse(image);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting image: {ImageId} for property: {PropertyId}", imageId, propertyId);
                var errorResponse = ApiResponse<PropertyImageResponse>.ErrorResponse("An error occurred while retrieving the image", 500);
                return StatusCode(500, errorResponse);
            }
        }
    }

}
