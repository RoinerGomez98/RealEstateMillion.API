using FluentValidation;
using RealEstateMillion.Application.DTOs.PropertyImage;
using RealEstateMillion.Domain.Interfaces;

namespace RealEstateMillion.Application.Validators
{
    public class AddImageValidator : AbstractValidator<AddImageRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddImageValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.PropertyId)
                .Must(id => id != Guid.Empty).WithMessage("Property ID is required");

            RuleFor(x => x.File)
                .NotEmpty().WithMessage("File path is required")
                .MaximumLength(500).WithMessage("File path cannot exceed 500 characters")
                .Must(BeValidImageExtension).WithMessage("File must be a valid image format (jpg, jpeg, png, gif, webp)");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");
        }
        private bool BeValidImageExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return validExtensions.Contains(extension);
        }
    }
}
