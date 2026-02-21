using Microsoft.EntityFrameworkCore;
using EcoWattAPI.Models;

namespace EcoWattAPI.Data
{
    public class EcoWattContext : DbContext
    {
        public EcoWattContext(DbContextOptions<EcoWattContext> options) : base(options)
        {
        }

        // DB Tables
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Usage> Usages { get; set; }
        public DbSet<Billing> Billings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1 Tariff → Many Customers
            modelBuilder.Entity<Tariff>()
                .HasMany(t => t.Customers)
                .WithOne(c => c.Tariff)
                .HasForeignKey(c => c.TariffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique indexes for data integrity
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Tariff>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // 1 Customer → Many Usage entries
            modelBuilder.Entity<Usage>()
                .HasOne(u => u.Customer)
                .WithMany(c => c.Usages)
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1 Customer → Many Billing entries
            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Billings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
