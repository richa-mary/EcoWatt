using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoWattAPI.Models
{
    public class Customer
    {
        [Key]  
        public int CustomerId { get; set; }   // Primary key

        [Required]
        public string FirstName { get; set; } = null!; // Required field

        [Required]
        public string LastName { get; set; } = null!;  // Required field

        [Required]
        public string Email { get; set; } = null!;     // Unique email (you can enforce unique in DB)

        public string Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        public string Postcode { get; set; } = null!;

        // Foreign Key → Tariff
        [ForeignKey("Tariff")]
        public int TariffId { get; set; }

        public Tariff Tariff { get; set; }    // Navigation property

        // Navigation collections
        public ICollection<Usage> Usages { get; set; } = new List<Usage>();
        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
    }
}
