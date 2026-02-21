using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public interface IUsageService
    {
        Task<List<Usage>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
        Task<Usage> CreateAsync(Usage usage, CancellationToken ct = default);
    }
}