using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class TweetDTO
    {
        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = null!;
    }
}