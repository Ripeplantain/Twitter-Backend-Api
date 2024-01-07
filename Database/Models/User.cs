using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;



namespace TwitterApi.Models
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Password cannot be longer than 50 characters")]
        public string Password { get; set; } = string.Empty;

        public UserProfile UserProfile { get; set; } = null!;
    }
}