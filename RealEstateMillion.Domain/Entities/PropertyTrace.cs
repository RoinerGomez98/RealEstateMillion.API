using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RealEstateMillion.Domain.Common;

namespace RealEstateMillion.Domain.Entities
{
    public class PropertyTrace : BaseEntity
    {
        public DateTime DateSale { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }


        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TransactionType { get; set; }

        [MaxLength(100)]
        public string? AgentName { get; set; }

        [MaxLength(100)]
        public string? BuyerName { get; set; }

        [MaxLength(100)]
        public string? SellerName { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? CommissionRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CommissionAmount { get; set; }

        public Guid PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;
    }
}
