using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoWattAPI.Models
{
    public class Usage
    {
        [Key]
        public int UsageId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;  // Navigation

        public DateTime Date { get; set; }      // Meter reading date/time

        // These are meter readings (cumulative). Billing will use differences between readings.
        public decimal ElectricityReading { get; set; }

        public decimal GasReading { get; set; }
    }
}
