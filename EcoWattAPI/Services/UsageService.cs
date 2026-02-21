using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Data;
using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public class UsageService : IUsageService
    {
        private readonly EcoWattContext _context;
        private readonly IBillingService _billingService;

        public UsageService(EcoWattContext context, IBillingService billingService)
        {
            _context = context;
            _billingService = billingService;
        }

        public async Task<List<Usage>> GetByCustomerAsync(int customerId, CancellationToken ct = default) =>
            await _context.Usages
                .Where(u => u.CustomerId == customerId)
                .OrderByDescending(u => u.Date)
                .ToListAsync(ct);

        public async Task<Usage> CreateAsync(Usage usage, CancellationToken ct = default)
        {
            if (usage.Date > DateTime.UtcNow)
                throw new ArgumentException("Cannot record future usage readings");
            if (usage.ElectricityReading < 0 || usage.GasReading < 0)
                throw new ArgumentException("Readings cannot be negative");

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == usage.CustomerId, ct);
            if (!customerExists)
                throw new ArgumentException($"Customer {usage.CustomerId} does not exist");

            var duplicate = await _context.Usages
                .AnyAsync(u => u.CustomerId == usage.CustomerId && u.Date.Date == usage.Date.Date, ct);
            if (duplicate)
                throw new ArgumentException("A usage record already exists for this customer on this date");

            var previous = await _context.Usages
                .Where(u => u.CustomerId == usage.CustomerId && u.Date < usage.Date)
                .OrderByDescending(u => u.Date)
                .FirstOrDefaultAsync(ct);

            if (previous != null)
            {
                if (usage.ElectricityReading < previous.ElectricityReading)
                    throw new ArgumentException("Electricity reading cannot decrease");
                if (usage.GasReading < previous.GasReading)
                    throw new ArgumentException("Gas reading cannot decrease");
            }

            _context.Usages.Add(usage);
            await _context.SaveChangesAsync(ct);

            try
            {
                await _billingService.GenerateBillForUsageAsync(usage, ct);
            }
            catch
            {
                // optionally log
            }

            return usage;
        }
    }
}