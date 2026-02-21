using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Data;
using EcoWattAPI.Models;

namespace EcoWattAPI.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly EcoWattContext _context;
        private const decimal DaysInMonthEstimate = 30m;

        public QuoteService(EcoWattContext context) => _context = context;

        public async Task<QuoteResult?> CalculateQuoteForCustomerAsync(
            int customerId,
            decimal? expectedMonthlyElectricity = null,
            decimal? expectedMonthlyGas = null,
            CancellationToken ct = default)
        {
            var customer = await _context.Customers
                .Include(c => c.Tariff)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);

            if (customer?.Tariff == null) return null;

            decimal? monthlyElec = expectedMonthlyElectricity;
            decimal? monthlyGas = expectedMonthlyGas;

            if (!monthlyElec.HasValue || !monthlyGas.HasValue)
            {
                var usages = await _context.Usages
                    .Where(u => u.CustomerId == customerId)
                    .OrderBy(u => u.Date)
                    .ToListAsync(ct);

                if (usages.Count >= 2)
                {
                    var totalDays = 0m;
                    var totalElec = 0m;
                    var totalGas = 0m;

                    for (int i = 1; i < usages.Count; i++)
                    {
                        var prev = usages[i - 1];
                        var curr = usages[i];
                        var days = (decimal)(curr.Date - prev.Date).TotalDays;
                        if (days <= 0) continue;

                        var elecDelta = curr.ElectricityReading - prev.ElectricityReading;
                        var gasDelta = curr.GasReading - prev.GasReading;
                        if (elecDelta < 0 || gasDelta < 0) continue; // skip bad data

                        totalDays += days;
                        totalElec += elecDelta;
                        totalGas += gasDelta;
                    }

                    if (totalDays > 0)
                    {
                        var avgDailyElec = totalElec / totalDays;
                        var avgDailyGas = totalGas / totalDays;
                        if (!monthlyElec.HasValue) monthlyElec = avgDailyElec * DaysInMonthEstimate;
                        if (!monthlyGas.HasValue) monthlyGas = avgDailyGas * DaysInMonthEstimate;
                    }
                }
            }

            if (!monthlyElec.HasValue || !monthlyGas.HasValue) return null;

            return CalculateForTariff(customer.Tariff, monthlyElec.Value, monthlyGas.Value);
        }

        public async Task<QuoteResult?> CalculateQuoteForTariffAsync(
            int tariffId,
            decimal monthlyElectricity,
            decimal monthlyGas,
            CancellationToken ct = default)
        {
            var tariff = await _context.Tariffs.FirstOrDefaultAsync(t => t.TariffId == tariffId, ct);
            if (tariff == null) return null;

            return CalculateForTariff(tariff, monthlyElectricity, monthlyGas);
        }

        private QuoteResult CalculateForTariff(Tariff tariff, decimal monthlyElectricity, decimal monthlyGas)
        {
            var standing = (tariff.ElecStandingCharge + tariff.GasStandingCharge) * DaysInMonthEstimate;
            var monthlyAmount = (monthlyElectricity * tariff.ElecUnitRate) +
                                (monthlyGas * tariff.GasUnitRate) +
                                standing;

            var monthlyRounded = Math.Round(monthlyAmount, 2);
            var annualRounded = Math.Round(monthlyAmount * 12, 2);

            return new QuoteResult(monthlyRounded, annualRounded, tariff.TariffId, tariff.Name);
        }
    }
}