using Microsoft.AspNetCore.Mvc;
using EcoWattAPI.Services;
using EcoWattAPI.Models;

namespace EcoWattAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffsController : ControllerBase
    {
        private readonly ITariffService _tariffService;

        public TariffsController(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        // GET: api/tariffs
        [HttpGet]
        public async Task<ActionResult<List<Tariff>>> GetAllTariffs(CancellationToken ct)
        {
            var tariffs = await _tariffService.GetAllAsync(ct);
            return Ok(tariffs);
        }

        // GET: api/tariffs/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tariff>> GetTariff(int id, CancellationToken ct)
        {
            var tariff = await _tariffService.GetByIdAsync(id, ct);
            if (tariff == null) return NotFound(new { message = "Tariff not found" });
            return Ok(tariff);
        }
    }
}