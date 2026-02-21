using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoWattAPI.Models
{
    public class Billing
    {
        [Key]
        public int BillingId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public DateTime BillDate { get; set; }   // Monthly bill date

        public decimal Amount { get; set; }

        public string Status { get; set; } = "Pending";       // Paid / Due / Overdue
    }
}
