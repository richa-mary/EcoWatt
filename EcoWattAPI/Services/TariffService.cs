using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Data;
using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public class TariffService : ITariffService
    {
        private readonly EcoWattContext _context;
        public TariffService(EcoWattContext context) => _context = context;

        public async Task<List<Tariff>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Tariffs.ToListAsync(ct);

        public async Task<Tariff?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Tariffs.FindAsync(new object[] { id }, ct);
    }
}