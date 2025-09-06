using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Application.Mappings;
using RealEstateMillion.Application.Services.Implementations;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Enums;
using RealEstateMillion.Domain.Interfaces;
using System.Linq.Expressions;

namespace RealEstateMillion.Tests.Services
{

    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPropertyRepository> _propertyRepositoryMock;
        private Mock<IOwnerRepository> _ownerRepositoryMock;
        private Mock<IPropertyTraceRepository> _propertyTraceRepositoryMock;
        private Mock<ILogger<PropertyService>> _loggerMock;
        private IMapper _mapper;
        private PropertyService _propertyService;

        [SetUp]
        public void SetUp()
        {

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _ownerRepositoryMock = new Mock<IOwnerRepository>();
            _propertyTraceRepositoryMock = new Mock<IPropertyTraceRepository>();
            _loggerMock = new Mock<ILogger<PropertyService>>();


            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyMappingProfile>();
            }, loggerFactory);

            _mapper = configuration.CreateMapper();

            _unitOfWorkMock.Setup(x => x.Properties).Returns(_propertyRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.Owners).Returns(_ownerRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.PropertyTraces).Returns(_propertyTraceRepositoryMock.Object);


            _propertyService = new PropertyService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
        }

        [Test]
        public async Task CreatePropertyAsync_WithValidData_ShouldReturnSuccessResponse()
        {
            var ownerId = Guid.NewGuid();

            var owner = new Owner
            {
                Id = ownerId,
                Name = "John Doe",
                Email = "john@example.com",
                Address = "123 Main St"
            };

            var request = new CreatePropertyRequest
            {
                Name = "Beautiful House",
                Address = "456 Oak Street",
                Price = 250000m,
                CodeInternal = "PROP001",
                Year = 2020,
                PropertyType = PropertyType.House,
                OwnerId = ownerId,
                Bedrooms = 3,
                Bathrooms = 2,
                City = "Miami",
                State = "FL"
            };

            _ownerRepositoryMock.Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _propertyRepositoryMock.Setup(x => x.CodeInternalExistsAsync("PROP001", null))
                .ReturnsAsync(false);

            _propertyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Property>()))
                .ReturnsAsync((Property p) => p);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Property, object>>[]>()))
                .ReturnsAsync((Guid id, Expression<Func<Property, object>>[] includes) => new Property
                {
                    Id = id,
                    Name = request.Name,
                    Address = request.Address,
                    Price = request.Price,
                    CodeInternal = request.CodeInternal,
                    Year = request.Year,
                    PropertyType = request.PropertyType,
                    OwnerId = request.OwnerId,
                    Owner = owner,
                    PropertyImages = []
                });

            _propertyTraceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PropertyTrace>()))
                .ReturnsAsync((PropertyTrace pt) => pt);


            var result = await _propertyService.CreatePropertyAsync(request);


            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(request.Name);
            result.Data.Address.Should().Be(request.Address);
            result.Data.Price.Should().Be(request.Price);
            result.Data.CodeInternal.Should().Be(request.CodeInternal);

            _ownerRepositoryMock.Verify(x => x.GetByIdAsync(ownerId), Times.Once);
            _propertyRepositoryMock.Verify(x => x.CodeInternalExistsAsync("PROP001", null), Times.Once);
            _propertyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Property>()), Times.Once);
            _propertyTraceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PropertyTrace>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task CreatePropertyAsync_WithInvalidOwner_ShouldReturnNotFoundResponse()
        {
            var ownerId = Guid.NewGuid();
            var request = new CreatePropertyRequest
            {
                Name = "Test Property",
                Address = "123 Test St",
                Price = 100000m,
                CodeInternal = "TEST001",
                Year = 2020,
                PropertyType = PropertyType.House,
                OwnerId = ownerId
            };

            _ownerRepositoryMock.Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync((Owner?)null);


            var result = await _propertyService.CreatePropertyAsync(request);


            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Owner not found");
            result.Data.Should().BeNull();

            _ownerRepositoryMock.Verify(x => x.GetByIdAsync(ownerId), Times.Once);
            _propertyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Property>()), Times.Never);
        }

        [Test]
        public async Task CreatePropertyAsync_WithDuplicateCode_ShouldReturnBadRequestResponse()
        {
            var ownerId = Guid.NewGuid();
            var owner = new Owner { Id = ownerId, Name = "John Doe" };
            var request = new CreatePropertyRequest
            {
                Name = "Test Property",
                Address = "123 Test St",
                Price = 100000m,
                CodeInternal = "EXISTING001",
                Year = 2020,
                PropertyType = PropertyType.House,
                OwnerId = ownerId
            };

            _ownerRepositoryMock.Setup(x => x.GetByIdAsync(ownerId))
                .ReturnsAsync(owner);

            _propertyRepositoryMock.Setup(x => x.CodeInternalExistsAsync("EXISTING001", null))
                .ReturnsAsync(true);

            var result = await _propertyService.CreatePropertyAsync(request);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Property code already exists");

            _propertyRepositoryMock.Verify(x => x.CodeInternalExistsAsync("EXISTING001", null), Times.Once);
            _propertyRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Property>()), Times.Never);
        }

        [Test]
        public async Task ChangePriceAsync_WithValidData_ShouldReturnSuccessResponse()
        {
            var ownerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                Name = "Test Property",
                Price = 200000m,
                CodeInternal = "PROP001",
                Owner = new Owner { Id = ownerId, Name = "John Doe" },
                PropertyImages = []
            };

            var request = new ChangePriceRequest
            {
                NewPrice = 250000m,
                Reason = "Market appreciation"
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()))
                .ReturnsAsync(property);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _propertyTraceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PropertyTrace>()))
                .ReturnsAsync((PropertyTrace pt) => pt);

            var result = await _propertyService.ChangePriceAsync(propertyId, request);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Price.Should().Be(250000m);

            property.Price.Should().Be(250000m); 

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.Properties.Update(property), Times.Once);
            _propertyTraceRepositoryMock.Verify(x => x.AddAsync(It.Is<PropertyTrace>(pt =>
                pt.PropertyId == propertyId &&
                pt.Value == 250000m &&
                pt.TransactionType == "Price Change")), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPropertiesWithFiltersAsync_WithValidFilters_ShouldReturnPagedResults()
        {
            var propertyId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var properties = new List<Property>
        {
            new() {
                Id = propertyId,
                Name = "Property 1",
                Price = 200000m,
                City = "Miami",
                State = "FL",
                Owner = new Owner { Id = ownerId, Name = "Owner 1" },
                PropertyImages = []
            },
            new() {
                Id = propertyId,
                Name = "Property 2",
                Price = 300000m,
                City = "Miami",
                State = "FL",
                Owner = new Owner { Id = ownerId, Name = "Owner 2" },
                PropertyImages = []
            }
        };

            var filters = new PropertyFiltersRequest
            {
                PageNumber = 1,
                PageSize = 10,
                City = "Miami",
                MinPrice = 150000m,
                MaxPrice = 350000m
            };

            _propertyRepositoryMock.Setup(x => x.GetPropertiesWithFiltersAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<PropertyType?>(), It.IsAny<PropertyStatus?>(), It.IsAny<ListingType?>(), It.IsAny<PropertyCondition?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((properties, 2));

            var result = await _propertyService.GetPropertiesWithFiltersAsync(filters);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Items.Should().HaveCount(2);
            result.Data.TotalCount.Should().Be(2);
            result.Data.PageNumber.Should().Be(1);
            result.Data.PageSize.Should().Be(10);

            _propertyRepositoryMock.Verify(x => x.GetPropertiesWithFiltersAsync(
                filters.PageNumber, filters.PageSize, filters.SearchTerm, filters.MinPrice, filters.MaxPrice,
                filters.PropertyType, filters.Status, filters.ListingType, filters.Condition,
                filters.MinBedrooms, filters.MaxBedrooms, filters.MinBathrooms, filters.MaxBathrooms,
                filters.MinSquareFeet, filters.MaxSquareFeet, filters.City, filters.State, filters.ZipCode,
                filters.Neighborhood, filters.HasPool, filters.HasGarden, filters.HasGarage,
                filters.HasFireplace, filters.PetsAllowed, filters.MinYear, filters.MaxYear,
                filters.SortBy, filters.SortDescending), Times.Once);
        }

        [Test]
        public async Task UpdatePropertyAsync_WithValidData_ShouldReturnSuccessResponse()
        {
            var propertyId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            var property = new Property
            {
                Id = propertyId,
                Name = "Original Name",
                Address = "Original Address",
                Price = 200000m,
                CodeInternal = "PROP001",
                Owner = new Owner { Id = ownerId, Name = "John Doe" },
                PropertyImages = []
            };

            var request = new UpdatePropertyRequest
            {
                Name = "Updated Name",
                Price = 250000m,
                Bedrooms = 4,
                City = "Updated City"
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()))
                .ReturnsAsync(property);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _propertyTraceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PropertyTrace>()))
                .ReturnsAsync((PropertyTrace pt) => pt);


            var result = await _propertyService.UpdatePropertyAsync(propertyId, request);


            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();


            property.Name.Should().Be("Updated Name");
            property.Price.Should().Be(250000m);
            property.Bedrooms.Should().Be(4);
            property.City.Should().Be("Updated City");

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.Properties.Update(property), Times.Once);
            _propertyTraceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PropertyTrace>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task GetPropertyByIdAsync_WithExistingId_ShouldReturnProperty()
        {
            var propertyId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                Name = "Test Property",
                Price = 200000m,
                Owner = new Owner { Id = ownerId, Name = "John Doe" },
                PropertyImages = [],
                PropertyTraces = []
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()))
                .ReturnsAsync(property);


            var result = await _propertyService.GetPropertyByIdAsync(propertyId);


            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(propertyId);
            result.Data.Name.Should().Be("Test Property");

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()), Times.Once);
        }

        [Test]
        public async Task GetPropertyByIdAsync_WithNonExistentId_ShouldReturnNotFound()
        {
            var propertyId = Guid.NewGuid();
            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<Expression<Func<Property, object>>[]>()))
                .ReturnsAsync((Property?)null);

            var result = await _propertyService.GetPropertyByIdAsync(propertyId);


            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Property not found");
            result.Data.Should().BeNull();
        }

        [Test]
        public async Task DeletePropertyAsync_WithExistingId_ShouldReturnSuccess()
        {
            var propertyId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                Name = "Test Property",
                CodeInternal = "PROP001"
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId))
                .ReturnsAsync(property);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var result = await _propertyService.DeletePropertyAsync(propertyId);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().BeTrue();

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(propertyId), Times.Once);
            _unitOfWorkMock.Verify(x => x.Properties.Remove(property), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
