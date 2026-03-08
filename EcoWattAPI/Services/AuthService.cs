using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EcoWattAPI.Data;
using EcoWattAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcoWattAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly EcoWattContext _context;
        private readonly IConfiguration _config;

        public AuthService(EcoWattContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
        {
            var postcode = req.Postcode.ToUpper().Replace(" ", "");

            var exists = await _context.Customers.AnyAsync(c => c.Email == req.Email, ct);
            if (exists) return null;

            CreatePasswordHash(req.Password, out string hash, out string salt);

            var customer = new Customer
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Phone = req.Phone ?? string.Empty,
                Address = req.Address,
                Postcode = postcode,
                TariffId = req.TariffId > 0 ? req.TariffId : 1,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(ct);

            return BuildResponse(customer);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == req.Email, ct);

            if (customer == null) return null;
            if (string.IsNullOrEmpty(customer.PasswordHash) || string.IsNullOrEmpty(customer.PasswordSalt))
                return null;

            if (!VerifyPassword(req.Password, customer.PasswordHash, customer.PasswordSalt))
                return null;

            return BuildResponse(customer);
        }

        // ──────────────── helpers ────────────────

        private static void CreatePasswordHash(string password, out string hash, out string salt)
        {
            using var hmac = new HMACSHA512();
            var saltBytes = hmac.Key;
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                var saltBytes = Convert.FromBase64String(storedSalt);
                var hashBytes = Convert.FromBase64String(storedHash);
                using var hmac = new HMACSHA512(saltBytes);
                var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return CryptographicOperations.FixedTimeEquals(computed, hashBytes);
            }
            catch { return false; }
        }

        private AuthResponse BuildResponse(Customer customer)
        {
            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 1440;

            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.CustomerId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                new Claim("customerId", customer.CustomerId.ToString()),
                new Claim("firstName", customer.FirstName),
                new Claim("tariffId", customer.TariffId.ToString())
            };

            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var token = new JwtSecurityToken(issuer, audience, claims,
                expires: expiry, signingCredentials: creds);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                TariffId = customer.TariffId,
                ExpiresAt = expiry
            };
        }
    }
}
