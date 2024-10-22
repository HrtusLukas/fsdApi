using Microsoft.EntityFrameworkCore;
using fsdApi.Models;

namespace fsdApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }  
    }
}
