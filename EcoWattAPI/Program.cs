using EcoWattAPI.Data;
using EcoWattAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<EcoWattContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITariffService, TariffService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IUsageService, UsageService>();
builder.Services.AddScoped<IBillingService, BillingService>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.WriteIndented = false;
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ✅ CORS (must be registered here)
builder.Services.AddCors(options =>
{
    options.AddPolicy("EcoWattCors", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Middleware order matters
app.UseRouting();
app.UseCors("EcoWattCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
