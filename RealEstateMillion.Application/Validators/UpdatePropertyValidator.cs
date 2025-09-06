using FluentValidation;
using RealEstateMillion.Application.DTOs.Property;

namespace RealEstateMillion.Application.Validators
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .When(x => x.Price.HasValue);

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
