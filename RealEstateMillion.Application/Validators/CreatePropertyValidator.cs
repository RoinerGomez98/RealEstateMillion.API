using FluentValidation;
using RealEstateMillion.Application.DTOs.Property;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Application.Validators
{
    public class CreatePropertyValidator : AbstractValidator<CreatePropertyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePropertyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Property name is required")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.CodeInternal)
                .NotEmpty().WithMessage("Internal code is required")
                .MaximumLength(20).WithMessage("Internal code cannot exceed 20 characters");

            RuleFor(x => x.Year)
                .InclusiveBetween(1800, 2030).WithMessage("Year must be between 1800 and 2030");

            RuleFor(x => x.OwnerId)
                  .Must(id => id != Guid.Empty).WithMessage("Owner ID is required");

            RuleFor(x => x.Bedrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bedrooms cannot be negative")
                .When(x => x.Bedrooms.HasValue);

            RuleFor(x => x.Bathrooms)
                .GreaterThanOrEqualTo(0).WithMessage("Bathrooms cannot be negative")
                .When(x => x.Bathrooms.HasValue);

            RuleFor(x => x.SquareFeet)
                .GreaterThan(0).WithMessage("Square feet must be greater than 0")
                .When(x => x.SquareFeet.HasValue);

            RuleFor(x => x.ZipCode)
                .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid ZIP code format")
                .When(x => !string.IsNullOrEmpty(x.ZipCode));

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
                .When(x => x.Longitude.HasValue);
        }
    }
}
