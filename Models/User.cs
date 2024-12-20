using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } ///to store the hashed password

        [Required]
        public string Role { get; set; } = "User"; // Default role is 'User'
    }
}
