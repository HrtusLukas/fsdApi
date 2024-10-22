using System.ComponentModel.DataAnnotations;

namespace fsdApi.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; } // New property
        public string City { get; set; }    // New property
        public DateTime Created { get; set; }
    }
}
