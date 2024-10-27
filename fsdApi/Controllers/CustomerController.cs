using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using fsdApi.Models;
using fsdApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace fsdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public CustomerController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customer.ToListAsync();
        }

        [HttpPost("register")]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] Customer customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Password))
            {
                return BadRequest("Invalid data.");
            }

            customer.Created = DateTime.UtcNow;
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(customer);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid profile data.");
            }

            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId == null)
            {
                return Unauthorized();
            }

            var customer = await _context.Customer.FindAsync(int.Parse(customerId));
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Update the customer properties
            customer.FirstName = model.FirstName ?? customer.FirstName;
            customer.LastName = model.LastName ?? customer.LastName;
            customer.Email = model.Email ?? customer.Email;
            customer.Country = model.Country ?? customer.Country;
            customer.City = model.City ?? customer.City;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public class UpdateProfileModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var customer = await _context.Customer
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);

            if (customer == null)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(customer);

            // Create a sanitized customer object (excluding sensitive data)
            var customerResponse = new
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                // Assuming you have these properties in your Customer model
                Created = customer.Created,
                // Add any other non-sensitive properties you want to return
                // Don't include Password or other sensitive fields
            };

            // Return both token and customer data
            return Ok(new
            {
                Token = token,
                Customer = customerResponse
            });
        }

        private string GenerateJwtToken(Customer customer)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, customer.Email),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),  // Add user ID to claims
                // Add any other claims you need
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}