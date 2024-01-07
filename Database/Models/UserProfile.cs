using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;



namespace TwitterApi.Models
{
    public class UserProfile
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Fullname is required")]
        [StringLength(50, ErrorMessage = "Fullname cannot be longer than 50 characters")]
        public string Fullname { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Bio { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Gender { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}