using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public interface IBillingService
    {
        Task GenerateBillForUsageAsync(Usage newUsage, CancellationToken ct = default);
        Task<List<Billing>> GetBillsForCustomerAsync(int customerId, CancellationToken ct = default);
    }
}