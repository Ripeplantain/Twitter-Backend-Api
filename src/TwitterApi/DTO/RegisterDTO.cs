using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string username { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string fullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string email { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;

        [Required]
        public string bio { get; set; } = null!;

        [Required]
        public string location { get; set; } = null!;
    }
}