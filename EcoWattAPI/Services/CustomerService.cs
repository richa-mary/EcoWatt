using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Data;
using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly EcoWattContext _context;
        public CustomerService(EcoWattContext context) => _context = context;

        public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Customers.Include(c => c.Tariff).ToListAsync(ct);

        public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Customers.Include(c => c.Tariff).FirstOrDefaultAsync(c => c.CustomerId == id, ct);

        public async Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(ct);
            return customer;
        }

        public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
        {
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var customer = await _context.Customers.FindAsync(new object[] { id }, ct);
            if (customer == null) return;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            _context.Customers.AnyAsync(c => c.CustomerId == id, ct);
    }
}