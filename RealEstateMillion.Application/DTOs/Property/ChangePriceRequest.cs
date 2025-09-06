using System.ComponentModel.DataAnnotations;

namespace RealEstateMillion.Application.DTOs.Property
{
    public class ChangePriceRequest
    {
        [Required(ErrorMessage = "New price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal NewPrice { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}
