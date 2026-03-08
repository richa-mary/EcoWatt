using EcoWattAPI.Data;
using EcoWattAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoWattAPI
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(EcoWattContext context)
        {
            // Only seed if no tariffs exist
            if (await context.Tariffs.AnyAsync()) return;

            var tariffs = new List<Tariff>
            {
                new Tariff
                {
                    Name = "Fixed 12-Month",
                    ElecUnitRate = 0.2850m,       // £0.285 per kWh
                    ElecStandingCharge = 0.5900m,  // £0.59 per day
                    GasUnitRate = 0.0720m,          // £0.072 per kWh
                    GasStandingCharge = 0.2900m     // £0.29 per day
                },
                new Tariff
                {
                    Name = "Green 24-Month",
                    ElecUnitRate = 0.2780m,
                    ElecStandingCharge = 0.5500m,
                    GasUnitRate = 0.0700m,
                    GasStandingCharge = 0.2700m
                },
                new Tariff
                {
                    Name = "Variable (No Lock-in)",
                    ElecUnitRate = 0.3100m,
                    ElecStandingCharge = 0.6100m,
                    GasUnitRate = 0.0790m,
                    GasStandingCharge = 0.3200m
                }
            };

            context.Tariffs.AddRange(tariffs);
            await context.SaveChangesAsync();
        }
    }
}
