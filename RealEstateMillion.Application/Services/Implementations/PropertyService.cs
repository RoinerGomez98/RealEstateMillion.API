using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Application.Services.Interfaces;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Application.Services.Implementations
{
    public class PropertyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PropertyService> logger) : IPropertyService
    {
        public async Task<ApiResponse<PropertyResponse>> CreatePropertyAsync(CreatePropertyRequest request)
        {
            try
            {
                logger.LogInformation("Creating property with code: {CodeInternal}", request.CodeInternal);

                var owner = await unitOfWork.Owners.GetByIdAsync(request.OwnerId);
                if (owner == null)
                {
                    logger.LogWarning("Owner not found: {OwnerId}", request.OwnerId);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Owner not found", 404);
                }

                var codeExists = await unitOfWork.Properties.CodeInternalExistsAsync(request.CodeInternal, Guid.Empty);
                if (codeExists)
                {
                    logger.LogWarning("Property code already exists: {CodeInternal}", request.CodeInternal);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Property code already exists", 400);
                }

                var property = mapper.Map<Property>(request);
                property.Id = Guid.NewGuid();
                property.ListedDate = DateTime.UtcNow;
                property.Status = PropertyStatus.Available;

                await unitOfWork.Properties.AddAsync(property);
                await unitOfWork.SaveChangesAsync();

                var initialTrace = new PropertyTrace
                {
                    PropertyId = property.Id,
                    DateSale = DateTime.UtcNow,
                    Name = "Property Listed",
                    Value = property.Price,
                    Tax = property.PropertyTax ?? 0,
                    Description = "Initial property listing",
                    TransactionType = "Listing"
                };

                await unitOfWork.PropertyTraces.AddAsync(initialTrace);
                await unitOfWork.SaveChangesAsync();

                var createdProperty = await unitOfWork.Properties.GetByIdAsync(property.Id,
                    p => p.Owner, p => p.PropertyImages);

                var response = mapper.Map<PropertyResponse>(createdProperty);

                logger.LogInformation("Property created successfully: {PropertyId}", property.Id);
                return ApiResponse<PropertyResponse>.SuccessResponse(response, "Property created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating property with code: {CodeInternal}", request.CodeInternal);
                return ApiResponse<PropertyResponse>.ErrorResponse("An error occurred while creating the property", 500);
            }
        }

        public async Task<ApiResponse<PropertyResponse>> UpdatePropertyAsync(Guid propertyId, UpdatePropertyRequest request)
        {
            try
            {
                logger.LogInformation("Updating property: {PropertyId}", propertyId);

                var property = await unitOfWork.Properties.GetByIdAsync(propertyId, p => p.Owner, p => p.PropertyImages);
                if (property == null)
                {
                    logger.LogWarning("Property not found: {PropertyId}", propertyId);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Property not found", 404);
                }

                var changes = new List<string>();

                if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != property.Name)
                {
                    changes.Add($"Name: {property.Name} → {request.Name}");
                    property.Name = request.Name;
                }

                if (!string.IsNullOrWhiteSpace(request.Address) && request.Address != property.Address)
                {
                    changes.Add($"Address: {property.Address} → {request.Address}");
                    property.Address = request.Address;
                }

                if (request.Price.HasValue && request.Price != property.Price)
                {
                    changes.Add($"Price: {property.Price:C} → {request.Price:C}");
                    property.Price = request.Price.Value;
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                    property.Description = request.Description;

                if (request.PropertyType.HasValue)
                    property.PropertyType = request.PropertyType.Value;

                if (request.Status.HasValue)
                    property.Status = request.Status.Value;

                if (request.ListingType.HasValue)
                    property.ListingType = request.ListingType.Value;

                if (request.Condition.HasValue)
                    property.Condition = request.Condition.Value;

                if (request.Bedrooms.HasValue)
                    property.Bedrooms = request.Bedrooms.Value;

                if (request.Bathrooms.HasValue)
                    property.Bathrooms = request.Bathrooms.Value;

                if (request.HalfBathrooms.HasValue)
                    property.HalfBathrooms = request.HalfBathrooms.Value;

                if (request.ParkingSpaces.HasValue)
                    property.ParkingSpaces = request.ParkingSpaces.Value;

                if (request.SquareFeet.HasValue)
                    property.SquareFeet = request.SquareFeet.Value;

                if (request.LotSize.HasValue)
                    property.LotSize = request.LotSize.Value;

                if (!string.IsNullOrWhiteSpace(request.City))
                    property.City = request.City;

                if (!string.IsNullOrWhiteSpace(request.State))
                    property.State = request.State;

                if (!string.IsNullOrWhiteSpace(request.ZipCode))
                    property.ZipCode = request.ZipCode;

                if (!string.IsNullOrWhiteSpace(request.Neighborhood))
                    property.Neighborhood = request.Neighborhood;

                if (request.Latitude.HasValue)
                    property.Latitude = request.Latitude.Value;

                if (request.Longitude.HasValue)
                    property.Longitude = request.Longitude.Value;

                if (request.HasPool.HasValue)
                    property.HasPool = request.HasPool.Value;

                if (request.HasGarden.HasValue)
                    property.HasGarden = request.HasGarden.Value;

                if (request.HasGarage.HasValue)
                    property.HasGarage = request.HasGarage.Value;

                if (request.HasFireplace.HasValue)
                    property.HasFireplace = request.HasFireplace.Value;

                if (request.HasAirConditioning.HasValue)
                    property.HasAirConditioning = request.HasAirConditioning.Value;

                if (request.HasHeating.HasValue)
                    property.HasHeating = request.HasHeating.Value;

                if (request.IsFurnished.HasValue)
                    property.IsFurnished = request.IsFurnished.Value;

                if (request.PetsAllowed.HasValue)
                    property.PetsAllowed = request.PetsAllowed.Value;

                if (request.MonthlyRent.HasValue)
                    property.MonthlyRent = request.MonthlyRent.Value;

                if (request.PropertyTax.HasValue)
                    property.PropertyTax = request.PropertyTax.Value;

                if (request.HOAFees.HasValue)
                    property.HOAFees = request.HOAFees.Value;

                if (request.AvailableFrom.HasValue)
                    property.AvailableFrom = request.AvailableFrom.Value;

                unitOfWork.Properties.Update(property);
                await unitOfWork.SaveChangesAsync();

                if (changes.Count != 0)
                {
                    var updateTrace = new PropertyTrace
                    {
                        PropertyId = property.Id,
                        DateSale = DateTime.UtcNow,
                        Name = "Property Updated",
                        Value = property.Price,
                        Tax = property.PropertyTax ?? 0,
                        Description = $"Property updated. Changes: {string.Join(", ", changes)}",
                        TransactionType = "Update"
                    };

                    await unitOfWork.PropertyTraces.AddAsync(updateTrace);
                    await unitOfWork.SaveChangesAsync();
                }

                var response = mapper.Map<PropertyResponse>(property);

                logger.LogInformation("Property updated successfully: {PropertyId}", propertyId);
                return ApiResponse<PropertyResponse>.SuccessResponse(response, "Property updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating property: {PropertyId}", propertyId);
                return ApiResponse<PropertyResponse>.ErrorResponse("An error occurred while updating the property", 500);
            }
        }

        public async Task<ApiResponse<PropertyResponse>> ChangePriceAsync(Guid propertyId, ChangePriceRequest request)
        {
            try
            {
                logger.LogInformation("Changing price for property: {PropertyId} to {NewPrice}", propertyId, request.NewPrice);

                var property = await unitOfWork.Properties.GetByIdAsync(propertyId, p => p.Owner, p => p.PropertyImages);
                if (property == null)
                {
                    logger.LogWarning("Property not found: {PropertyId}", propertyId);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Property not found", 404);
                }

                var oldPrice = property.Price;
                property.Price = request.NewPrice;

                unitOfWork.Properties.Update(property);

                var priceChangeTrace = new PropertyTrace
                {
                    PropertyId = property.Id,
                    DateSale = DateTime.UtcNow,
                    Name = "Price Change",
                    Value = request.NewPrice,
                    Tax = property.PropertyTax ?? 0,
                    Description = $"Price changed from {oldPrice:C} to {request.NewPrice:C}. " +
                                 (string.IsNullOrWhiteSpace(request.Reason) ? "" : $"Reason: {request.Reason}"),
                    TransactionType = "Price Change"
                };

                await unitOfWork.PropertyTraces.AddAsync(priceChangeTrace);
                await unitOfWork.SaveChangesAsync();

                var response = mapper.Map<PropertyResponse>(property);

                logger.LogInformation("Price changed successfully for property: {PropertyId}", propertyId);
                return ApiResponse<PropertyResponse>.SuccessResponse(response, "Price updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error changing price for property: {PropertyId}", propertyId);
                return ApiResponse<PropertyResponse>.ErrorResponse("An error occurred while changing the price", 500);
            }
        }

        public async Task<ApiResponse<PropertyResponse>> GetPropertyByIdAsync(Guid propertyId)
        {
            try
            {
                var property = await unitOfWork.Properties.GetByIdAsync(propertyId,
                    p => p.Owner, p => p.PropertyImages, p => p.PropertyTraces);

                if (property == null)
                {
                    logger.LogWarning("Property not found: {PropertyId}", propertyId);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Property not found", 404);
                }

                var response = mapper.Map<PropertyResponse>(property);
                return ApiResponse<PropertyResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting property: {PropertyId}", propertyId);
                return ApiResponse<PropertyResponse>.ErrorResponse("An error occurred while retrieving the property", 500);
            }
        }

        public async Task<ApiResponse<PropertyResponse>> GetPropertyByCodeAsync(string codeInternal)
        {
            try
            {
                var property = await unitOfWork.Properties.GetByCodeInternalAsync(codeInternal);

                if (property == null)
                {
                    logger.LogWarning("Property not found with code: {CodeInternal}", codeInternal);
                    return ApiResponse<PropertyResponse>.ErrorResponse("Property not found", 404);
                }

                var response = mapper.Map<PropertyResponse>(property);
                return ApiResponse<PropertyResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting property by code: {CodeInternal}", codeInternal);
                return ApiResponse<PropertyResponse>.ErrorResponse("An error occurred while retrieving the property", 500);
            }
        }

        public async Task<ApiResponse<PagedResult<PropertyResponse>>> GetPropertiesWithFiltersAsync(PropertyFiltersRequest filters)
        {
            try
            {
                logger.LogInformation("Getting properties with filters - Page: {PageNumber}, Size: {PageSize}",
                    filters.PageNumber, filters.PageSize);

                var (properties, totalCount) = await unitOfWork.Properties.GetPropertiesWithFiltersAsync(
                    filters.PageNumber, filters.PageSize, filters.SearchTerm, filters.MinPrice, filters.MaxPrice,
                    filters.PropertyType, filters.Status, filters.ListingType, filters.Condition,
                    filters.MinBedrooms, filters.MaxBedrooms, filters.MinBathrooms, filters.MaxBathrooms,
                    filters.MinSquareFeet, filters.MaxSquareFeet, filters.City, filters.State, filters.ZipCode,
                    filters.Neighborhood, filters.HasPool, filters.HasGarden, filters.HasGarage,
                    filters.HasFireplace, filters.PetsAllowed, filters.MinYear, filters.MaxYear,
                    filters.SortBy, filters.SortDescending);

                var propertyResponses = mapper.Map<List<PropertyResponse>>(properties);

                var pagedResult = new PagedResult<PropertyResponse>
                {
                    Items = propertyResponses,
                    TotalCount = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };

                logger.LogInformation("Retrieved {Count} properties out of {Total}",
                    propertyResponses.Count, totalCount);

                return ApiResponse<PagedResult<PropertyResponse>>.SuccessResponse(pagedResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting properties with filters");
                return ApiResponse<PagedResult<PropertyResponse>>.ErrorResponse(
                    "An error occurred while retrieving properties", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeletePropertyAsync(Guid propertyId)
        {
            try
            {

                var property = await unitOfWork.Properties.GetByIdAsync(propertyId);
                if (property == null)
                {
                    logger.LogWarning("Property not found: {PropertyId}", propertyId);
                    return ApiResponse<bool>.ErrorResponse("Property not found", 404);
                }

                unitOfWork.Properties.Remove(property);
                await unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Property deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting property: {PropertyId}", propertyId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting the property", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<PropertyResponse>>> GetPropertiesByOwnerAsync(Guid ownerId)
        {
            try
            {
                var properties = await unitOfWork.Properties.GetByOwnerIdAsync(ownerId);
                var response = mapper.Map<IEnumerable<PropertyResponse>>(properties);

                return ApiResponse<IEnumerable<PropertyResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting properties for owner: {OwnerId}", ownerId);
                return ApiResponse<IEnumerable<PropertyResponse>>.ErrorResponse(
                    "An error occurred while retrieving owner's properties", 500);
            }
        }
    }

}
