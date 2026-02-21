using Microsoft.AspNetCore.Mvc;
using EcoWattAPI.Services;
using EcoWattAPI.Models;
using System.Text.RegularExpressions;

namespace EcoWattAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers(CancellationToken ct)
        {
            var customers = await _customerService.GetAllAsync(ct);
            return Ok(customers);
        }

        // GET: api/customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id, CancellationToken ct)
        {
            var customer = await _customerService.GetByIdAsync(id, ct);
            if (customer == null) return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
                    return BadRequest(new { message = "Invalid email format" });

                if (string.IsNullOrWhiteSpace(customer.Postcode))
                    return BadRequest(new { message = "Postal code is required" });

                customer.Postcode = customer.Postcode.ToUpper().Replace(" ", "");

                var created = await _customerService.CreateAsync(customer, ct);
                return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/customers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer, CancellationToken ct)
        {
            if (id != customer.CustomerId) return BadRequest(new { message = "ID mismatch" });

            try
            {
                if (!string.IsNullOrWhiteSpace(customer.Email) && !IsValidEmail(customer.Email))
                    return BadRequest(new { message = "Invalid email format" });

                if (!string.IsNullOrWhiteSpace(customer.Postcode))
                    customer.Postcode = customer.Postcode.ToUpper().Replace(" ", "");

                await _customerService.UpdateAsync(customer, ct);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/customers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id, CancellationToken ct)
        {
            try
            {
                await _customerService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}