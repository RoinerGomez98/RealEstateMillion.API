using System.ComponentModel.DataAnnotations;
using RealEstateMillion.Domain.Common;

namespace RealEstateMillion.Domain.Entities
{
    public class Owner : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [MaxLength(500)]
        public string? Photo { get; set; }

        public DateTime? Birthday { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? DocumentType { get; set; } 

        [MaxLength(50)]
        public string? DocumentNumber { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; } = "USA";


        public virtual ICollection<Property> Properties { get; set; } = [];
    }
}
