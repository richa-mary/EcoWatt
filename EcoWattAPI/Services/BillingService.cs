using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Data;
using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public class BillingService : IBillingService
    {
        private readonly EcoWattContext _context;
        public BillingService(EcoWattContext context) => _context = context;

        public async Task GenerateBillForUsageAsync(Usage newUsage, CancellationToken ct = default)
        {
            // calendar month of the new reading
            var periodStart = new DateTime(newUsage.Date.Year, newUsage.Date.Month, 1);
            var periodEnd = periodStart.AddMonths(1).AddTicks(-1);

            // skip if already billed this month
            var alreadyBilled = await _context.Billings.AnyAsync(
                b => b.CustomerId == newUsage.CustomerId &&
                     b.BillDate >= periodStart && b.BillDate <= periodEnd, ct);
            if (alreadyBilled) return;

            // baseline: last reading before this month
            var previous = await _context.Usages
                .Where(u => u.CustomerId == newUsage.CustomerId && u.Date < periodStart)
                .OrderByDescending(u => u.Date)
                .FirstOrDefaultAsync(ct);
            if (previous == null) return;

            // latest reading within this month (use current if none newer)
            var current = await _context.Usages
                .Where(u => u.CustomerId == newUsage.CustomerId &&
                            u.Date >= periodStart && u.Date <= periodEnd)
                .OrderByDescending(u => u.Date)
                .FirstOrDefaultAsync(ct) ?? newUsage;

            var elecDelta = Math.Max(0m, current.ElectricityReading - previous.ElectricityReading);
            var gasDelta  = Math.Max(0m, current.GasReading - previous.GasReading);

            var customer = await _context.Customers.Include(c => c.Tariff)
                .FirstOrDefaultAsync(c => c.CustomerId == newUsage.CustomerId, ct);
            if (customer?.Tariff == null) return;

            var t = customer.Tariff;
            var daysInMonth = DateTime.DaysInMonth(periodStart.Year, periodStart.Month);
            var standing = (t.ElecStandingCharge + t.GasStandingCharge) * daysInMonth;

            var amount = (elecDelta * t.ElecUnitRate) + (gasDelta * t.GasUnitRate) + standing;

            var bill = new Billing
            {
                CustomerId = newUsage.CustomerId,
                BillDate = periodEnd, // end of month
                Amount = Math.Round(amount, 2),
                Status = "Unpaid"
            };

            _context.Billings.Add(bill);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Billing>> GetBillsForCustomerAsync(int customerId, CancellationToken ct = default) =>
            await _context.Billings
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(ct);
    }
}