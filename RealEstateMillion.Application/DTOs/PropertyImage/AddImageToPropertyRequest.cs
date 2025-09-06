using System.ComponentModel.DataAnnotations;

namespace RealEstateMillion.Application.DTOs.PropertyImage
{
    public class AddImageToPropertyRequest
    {
        [Required(ErrorMessage = "File path is required")]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        public string File { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order cannot be negative")]
        public int DisplayOrder { get; set; } = 0;

        public bool IsPrimary { get; set; } = false;
    }
}
