using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public interface ITariffService
    {
        Task<List<Tariff>> GetAllAsync(CancellationToken ct = default);
        Task<Tariff?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}