using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoWattAPI.Models
{
    public class Quote
    {
        [Key]
        public int QuoteId { get; set; }

        [Required]
        public string Postcode { get; set; } = null!;

        public string? Address { get; set; }

        [Required]
        public decimal ElectricityUsage { get; set; }

        public decimal GasUsage { get; set; }

        [ForeignKey("Tariff")]
        public int TariffId { get; set; }     // Recommended tariff

        public Tariff Tariff { get; set; } = null!;    // Navigation

        public decimal EstimatedCost { get; set; }
    }
}
