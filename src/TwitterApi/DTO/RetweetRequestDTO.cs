using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class RetweetRequestDTO
    {
        [Required]
        public int tweetId { get; set; }

        public string caption { get; set; } = string.Empty;
    }
}