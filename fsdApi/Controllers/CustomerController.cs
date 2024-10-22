using Microsoft.AspNetCore.Mvc;
using fsdApi.Models;
using fsdApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fsdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _context;

        public CustomerController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customer.ToListAsync();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] Customer customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Password))
            {
                return BadRequest("Invalid data.");
            }

            customer.Created = DateTime.UtcNow; // Ensure the DateTime is in UTC
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }
    }
}
