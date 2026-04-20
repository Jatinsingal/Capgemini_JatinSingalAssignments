using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? Username { get; set; }

        public string? MobileNumber { get; set; }

        public string? UserRole { get; set; } // Admin / InventoryManager
    }
}