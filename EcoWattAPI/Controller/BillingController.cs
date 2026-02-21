using Microsoft.AspNetCore.Mvc;
using EcoWattAPI.Models;
using EcoWattAPI.Services;

namespace EcoWattAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        // GET: api/billing/customer/5
        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<List<Billing>>> GetBillsForCustomer(int customerId, CancellationToken ct)
        {
            var bills = await _billingService.GetBillsForCustomerAsync(customerId, ct);
            if (!bills.Any()) return NotFound(new { message = "No bills found for this customer" });
            return Ok(bills);
        }
    }
}