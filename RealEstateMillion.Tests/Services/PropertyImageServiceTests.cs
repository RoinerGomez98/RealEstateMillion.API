using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Application.Mappings;
using RealEstateMillion.Application.Services.Implementations;
using RealEstateMillion.Domain.Entities;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Tests.Services
{
    [TestFixture]
    public class PropertyImageServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPropertyRepository> _propertyRepositoryMock;
        private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
        private Mock<ILogger<PropertyImageService>> _loggerMock;
        private IMapper _mapper;
        private PropertyImageService _propertyImageService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
            _loggerMock = new Mock<ILogger<PropertyImageService>>();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyMappingProfile>();
            }, loggerFactory);

            _mapper = configuration.CreateMapper();

            _unitOfWorkMock.Setup(x => x.Properties).Returns(_propertyRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.PropertyImages).Returns(_propertyImageRepositoryMock.Object);

            _propertyImageService = new PropertyImageService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
        }

        [Test]
        public async Task AddImageAsync_WithValidData_ShouldReturnSuccessResponse()
        {
            var prop = Guid.NewGuid();
            var property = new Property
            {
                Id = prop,
                Name = "Test Property",
                CodeInternal = "PROP001"
            };

            var request = new AddImageRequest
            {
                PropertyId = prop,
                File = "/uploads/test-image.jpg",
                Title = "Front View",
                Description = "Beautiful front view of the property",
                DisplayOrder = 1,
                IsPrimary = true
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(prop))
                .ReturnsAsync(property);

            _propertyImageRepositoryMock.Setup(x => x.DisablePrimaryImagesAsync(prop))
                .Returns(Task.CompletedTask);

            _propertyImageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PropertyImage>()))
                .ReturnsAsync((PropertyImage pi) => { pi.Id = prop; return pi; });

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var result = await _propertyImageService.AddImageAsync(request);

   
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data!.File.Should().Be("/uploads/test-image.jpg");
            result.Data.Title.Should().Be("Front View");
            result.Data.IsPrimary.Should().BeTrue();

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(prop), Times.Once);
            _propertyImageRepositoryMock.Verify(x => x.DisablePrimaryImagesAsync(prop), Times.Once);
            _propertyImageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PropertyImage>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddImageAsync_WithNonExistentProperty_ShouldReturnNotFound()
        {
            var prop = Guid.NewGuid();
            var request = new AddImageRequest
            {
                PropertyId = prop,
                File = "/uploads/test-image.jpg"
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(prop))
                .ReturnsAsync((Property?)null);


            var result = await _propertyImageService.AddImageAsync(request);

   
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Property not found");

            _propertyRepositoryMock.Verify(x => x.GetByIdAsync(prop), Times.Once);
            _propertyImageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PropertyImage>()), Times.Never);
        }

        [Test]
        public async Task AddImageAsync_WithAutoDisplayOrder_ShouldSetCorrectOrder()
        {
   
            var prop = Guid.NewGuid();
            var property = new Property { Id = prop, Name = "Test Property" };
            var request = new AddImageRequest
            {
                PropertyId = prop,
                File = "/uploads/test-image.jpg",
                DisplayOrder = 0 
            };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(prop))
                .ReturnsAsync(property);

            _propertyImageRepositoryMock.Setup(x => x.GetMaxDisplayOrderAsync(prop))
                .ReturnsAsync(5); 

            _propertyImageRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PropertyImage>()))
                .ReturnsAsync((PropertyImage pi) => { pi.Id = prop; return pi; });

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var result = await _propertyImageService.AddImageAsync(request);

   
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data!.DisplayOrder.Should().Be(6); 

            _propertyImageRepositoryMock.Verify(x => x.GetMaxDisplayOrderAsync(prop), Times.Once);
        }

        [Test]
        public async Task SetPrimaryImageAsync_WithValidImage_ShouldReturnSuccess()
        {
   
            var prop = Guid.NewGuid();
            var propImg = Guid.NewGuid();
            var image = new PropertyImage
            {
                Id = propImg,
                PropertyId = prop,
                File = "/uploads/test-image.jpg",
                IsPrimary = false
            };

            _propertyImageRepositoryMock.Setup(x => x.GetByIdAsync(prop))
                .ReturnsAsync(image);

            _propertyImageRepositoryMock.Setup(x => x.DisablePrimaryImagesAsync(prop))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var result = await _propertyImageService.SetPrimaryImageAsync(propImg);

   
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Data!.IsPrimary.Should().BeTrue();

            image.IsPrimary.Should().BeTrue(); 

            _propertyImageRepositoryMock.Verify(x => x.GetByIdAsync(propImg), Times.Once);
            _propertyImageRepositoryMock.Verify(x => x.DisablePrimaryImagesAsync(prop), Times.Once);
            _unitOfWorkMock.Verify(x => x.PropertyImages.Update(image), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteImageAsync_WithPrimaryImage_ShouldAssignNewPrimary()
        {
            var prop = Guid.NewGuid();
            var img1 = Guid.NewGuid();
            var imageToDelete = new PropertyImage
            {
                Id = img1,
                PropertyId = prop,
                IsPrimary = true,
                File = "/uploads/primary-image.jpg"
            };

            var remainingImages = new List<PropertyImage>
        {
            new() {
                Id = Guid.NewGuid(),
                PropertyId = prop,
                IsPrimary = false,
                File = "/uploads/other-image.jpg"
            }
        };

            _propertyImageRepositoryMock.Setup(x => x.GetByIdAsync(img1))
                .ReturnsAsync(imageToDelete);

            _propertyImageRepositoryMock.Setup(x => x.GetByPropertyIdAsync(prop))
                .ReturnsAsync(remainingImages);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var result = await _propertyImageService.DeleteImageAsync(img1);

   
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

            remainingImages.First().IsPrimary.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.PropertyImages.Remove(imageToDelete), Times.Once);
            _unitOfWorkMock.Verify(x => x.PropertyImages.Update(remainingImages.First()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task GetImagesByPropertyAsync_WithValidPropertyId_ShouldReturnImages()
        {
            var prop = Guid.NewGuid();
            var images = new List<PropertyImage>
        {
            new() {
                Id = Guid.NewGuid(),
                PropertyId = prop,
                File = "/uploads/image1.jpg",
                Title = "Image 1",
                IsPrimary = true
            },
            new() {
                Id = Guid.NewGuid(),
                PropertyId = prop,
                File = "/uploads/image2.jpg",
                Title = "Image 2",
                IsPrimary = false
            }
        };

            _propertyImageRepositoryMock.Setup(x => x.GetByPropertyIdAsync(prop))
                .ReturnsAsync(images);


            var result = await _propertyImageService.GetImagesByPropertyAsync(prop);

   
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data!.First().IsPrimary.Should().BeTrue();

            _propertyImageRepositoryMock.Verify(x => x.GetByPropertyIdAsync(prop), Times.Once);
        }
    }
}
