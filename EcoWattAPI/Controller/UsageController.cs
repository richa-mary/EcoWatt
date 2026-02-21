using Microsoft.AspNetCore.Mvc;
using EcoWattAPI.Models;
using EcoWattAPI.Services;

namespace EcoWattAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsageController : ControllerBase
    {
        private readonly IUsageService _usageService;

        public UsageController(IUsageService usageService)
        {
            _usageService = usageService;
        }

        // GET: api/usage/customer/5
        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<List<Usage>>> GetByCustomer(int customerId, CancellationToken ct)
        {
            var usages = await _usageService.GetByCustomerAsync(customerId, ct);
            return Ok(usages);
        }

        // POST: api/usage
        [HttpPost]
        public async Task<ActionResult<Usage>> PostUsage(Usage usage, CancellationToken ct)
        {
            try
            {
                var created = await _usageService.CreateAsync(usage, ct);
                return CreatedAtAction(nameof(GetByCustomer), new { customerId = created.CustomerId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}