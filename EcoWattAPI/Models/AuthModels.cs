using System.ComponentModel.DataAnnotations;

namespace EcoWattAPI.Models
{
    public class RegisterRequest
    {
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, MinLength(8)] public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [Required] public string Postcode { get; set; } = null!;
        public int TariffId { get; set; } = 1;
    }

    public class LoginRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TariffId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
