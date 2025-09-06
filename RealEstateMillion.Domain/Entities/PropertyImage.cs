using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstateMillion.Domain.Common;

namespace RealEstateMillion.Domain.Entities
{
    public class PropertyImage : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string File { get; set; } = null!;

        public bool Enabled { get; set; } = true;


        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsPrimary { get; set; } = false;

        [MaxLength(50)]
        public string? FileType { get; set; } 

        public long? FileSizeBytes { get; set; }

        [MaxLength(500)]
        public string? ThumbnailPath { get; set; }


        public Guid PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;
    }
}
