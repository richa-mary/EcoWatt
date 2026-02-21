using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
        Task UpdateAsync(Customer customer, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}