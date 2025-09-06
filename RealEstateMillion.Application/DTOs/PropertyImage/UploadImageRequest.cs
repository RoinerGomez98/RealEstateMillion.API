using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateMillion.Application.DTOs.PropertyImage
{
    public class UploadImageRequest
    {
        [Required]
        public IFormFile file { get; set; }

        public string? title { get; set; }

        public string? description { get; set; }

        public int displayOrder { get; set; } = 0;

        public bool isPrimary { get; set; } = false;
    }
}
