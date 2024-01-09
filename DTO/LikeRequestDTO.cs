using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class LikeRequestDTO
    {
        [Required]
        public int tweetId { get; set; }
    }
}