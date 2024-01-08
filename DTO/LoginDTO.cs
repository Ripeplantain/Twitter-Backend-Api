using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(50)]
        public string username { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;
    }
}