using Microsoft.AspNetCore.Mvc;
using EcoWattAPI.Services;

namespace EcoWattAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        // GET: api/quote/customer/5?expectedMonthlyElectricity=250&expectedMonthlyGas=300
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetCustomerQuote(
            int customerId,
            [FromQuery] decimal? expectedMonthlyElectricity,
            [FromQuery] decimal? expectedMonthlyGas,
            CancellationToken ct)
        {
            var result = await _quoteService.CalculateQuoteForCustomerAsync(
                customerId,
                expectedMonthlyElectricity,
                expectedMonthlyGas,
                ct);

            if (result is null) return NotFound(new { message = "Quote could not be generated" });
            return Ok(result);
        }

        // GET: api/quote/tariff/3?monthlyElectricity=250&monthlyGas=300
        [HttpGet("tariff/{tariffId:int}")]
        public async Task<IActionResult> GetTariffQuote(
            int tariffId,
            [FromQuery] decimal monthlyElectricity,
            [FromQuery] decimal monthlyGas,
            CancellationToken ct)
        {
            if (monthlyElectricity < 0 || monthlyGas < 0)
                return BadRequest(new { message = "Monthly usage must be non-negative" });

            var result = await _quoteService.CalculateQuoteForTariffAsync(
                tariffId,
                monthlyElectricity,
                monthlyGas,
                ct);

            if (result is null) return NotFound(new { message = "Tariff not found" });
            return Ok(result);
        }
    }
}