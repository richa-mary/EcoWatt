using System.ComponentModel.DataAnnotations;

namespace EcoWattAPI.Models
{
    public class Tariff
    {
        [Key]
        public int TariffId { get; set; }   // Primary key
        
        [Required]
        public string Name { get; set; } = null!;    // Tariff name (1-year / 2-year / variable)

        // Electricity Pricing
        [Required]
        public decimal ElecUnitRate { get; set; }       // GBP per kWh
        [Required]
        public decimal ElecStandingCharge { get; set; } // GBP per day

        // Gas Pricing
        [Required]
        public decimal GasUnitRate { get; set; }        // GBP per kWh
        [Required]
        public decimal GasStandingCharge { get; set; }  // GBP per day

        // Navigation
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
