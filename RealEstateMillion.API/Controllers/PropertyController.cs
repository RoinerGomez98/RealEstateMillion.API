using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMillion.API.Attributes;
using RealEstateMillion.Application.Common;
using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Application.Services.Interfaces;

namespace RealEstateMillion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger) : ControllerBase
    {

        /// <summary>
        /// Create a new property
        /// </summary>
        /// <param name="request">Property creation details</param>
        /// <returns>Created property information</returns>
        /// <response code="200">Property created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Owner not found</response>
        /// <response code="409">Property code already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> CreateProperty(
            [FromBody] CreatePropertyRequest request)
        {
            logger.LogInformation("Creating property with code: {CodeInternal}", request.CodeInternal);

            var result = await propertyService.CreatePropertyAsync(request);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="request">Property update details</param>
        /// <returns>Updated property information</returns>
        /// <response code="200">Property updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> UpdateProperty(
            [FromRoute] Guid id,
            [FromBody] UpdatePropertyRequest request)
        {

            var result = await propertyService.UpdatePropertyAsync(id, request);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Change property price
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="request">Price change details</param>
        /// <returns>Property with updated price</returns>
        /// <response code="200">Price updated successfully</response>
        /// <response code="400">Invalid price data</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id:Guid}/price")]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> ChangePrice(
            [FromRoute] Guid id,
            [FromBody] ChangePriceRequest request)
        {
            logger.LogInformation("Changing price for property: {PropertyId} to {NewPrice}", id, request.NewPrice);

            var result = await propertyService.ChangePriceAsync(id, request);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get property by ID
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Property details</returns>
        /// <response code="200">Property found</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> GetProperty([FromRoute] Guid id)
        {
            logger.LogInformation("Getting property: {PropertyId}", id);

            var result = await propertyService.GetPropertyByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get property by internal code
        /// </summary>
        /// <param name="code">Property internal code</param>
        /// <returns>Property details</returns>
        /// <response code="200">Property found</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("code/{code}")]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PropertyResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> GetPropertyByCode([FromRoute] string code)
        {
            logger.LogInformation("Getting property by code: {CodeInternal}", code);

            var result = await propertyService.GetPropertyByCodeAsync(code);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get properties with advanced filtering and pagination
        /// </summary>
        /// <param name="filters">Filter criteria</param>
        /// <returns>Paginated list of properties</returns>
        /// <response code="200">Properties retrieved successfully</response>
        /// <response code="400">Invalid filter parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PropertyResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PropertyResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PropertyResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyResponse>>>> GetProperties(
            [FromQuery] PropertyFiltersRequest filters)
        {
            logger.LogInformation("Getting properties with filters - Page: {PageNumber}, Size: {PageSize}",
                filters.PageNumber, filters.PageSize);

            var result = await propertyService.GetPropertiesWithFiltersAsync(filters);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get properties by owner
        /// </summary>
        /// <param name="ownerId">Owner ID</param>
        /// <returns>List of properties owned by the specified owner</returns>
        /// <response code="200">Properties retrieved successfully</response>
        /// <response code="404">Owner not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("owner/{ownerId:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyResponse>>>> GetPropertiesByOwner(
            [FromRoute] Guid ownerId)
        {
            logger.LogInformation("Getting properties for owner: {OwnerId}", ownerId);

            var result = await propertyService.GetPropertiesByOwnerAsync(ownerId);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a property (soft delete)
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Property deleted successfully</response>
        /// <response code="404">Property not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProperty([FromRoute] Guid id)
        {
            logger.LogInformation("Deleting property: {PropertyId}", id);

            var result = await propertyService.DeletePropertyAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Get property statistics
        /// </summary>
        /// <returns>Property statistics and metrics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public ActionResult<ApiResponse<object>> GetPropertyStatistics()
        {
            try
            {
                logger.LogInformation("Getting property statistics");
                var stats = new
                {
                    TotalProperties = "This would come from service",
                    AvailableProperties = "This would come from service",
                    AveragePrice = "This would come from service",
                    PropertyTypeDistribution = "This would come from service",
                    RecentListings = "This would come from service"
                };

                var response = ApiResponse<object>.SuccessResponse(stats, "Statistics retrieved successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting property statistics");
                var errorResponse = ApiResponse<object>.ErrorResponse("An error occurred while retrieving statistics", 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Search properties by location
        /// </summary>
        /// <param name="city">City name</param>
        /// <param name="state">State abbreviation (optional)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Properties in the specified location</returns>
        /// <response code="200">Properties found</response>
        /// <response code="400">Invalid parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("location")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PropertyResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyResponse>>>> SearchPropertiesByLocation(
            [FromQuery, Required] string city,
            [FromQuery] string? state = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                logger.LogInformation("Searching properties by location - City: {City}, State: {State}", city, state);

                if (string.IsNullOrWhiteSpace(city))
                {
                    var badRequest = ApiResponse<IEnumerable<PropertyResponse>>.ErrorResponse("City parameter is required", 400);
                    return BadRequest(badRequest);
                }

                var filters = new PropertyFiltersRequest
                {
                    City = city,
                    State = state,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await propertyService.GetPropertiesWithFiltersAsync(filters);

                if (result.Success && result.Data != null)
                {
                    var locationResponse = ApiResponse<IEnumerable<PropertyResponse>>.SuccessResponse(
                        result.Data.Items,
                        $"Found {result.Data.TotalCount} properties in {city}{(!string.IsNullOrEmpty(state) ? $", {state}" : "")}");

                    return Ok(locationResponse);
                }

                return StatusCode(result.StatusCode, ApiResponse<IEnumerable<PropertyResponse>>.ErrorResponse(result.Message, result.StatusCode));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching properties by location - City: {City}, State: {State}", city, state);
                var errorResponse = ApiResponse<IEnumerable<PropertyResponse>>.ErrorResponse("An error occurred while searching properties", 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
