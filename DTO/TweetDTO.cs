using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class TweetDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = null!;

        public UserDTO? User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}