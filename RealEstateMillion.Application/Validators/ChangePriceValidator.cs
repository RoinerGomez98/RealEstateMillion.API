using FluentValidation;
using RealEstateMillion.Application.DTOs.Property;

namespace RealEstateMillion.Application.Validators
{
    public class ChangePriceValidator : AbstractValidator<ChangePriceRequest>
    {
        public ChangePriceValidator()
        {
            RuleFor(x => x.NewPrice)
                .GreaterThan(0).WithMessage("New price must be greater than 0");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Reason));
        }
    }
}
