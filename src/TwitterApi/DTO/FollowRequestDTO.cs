using System.ComponentModel.DataAnnotations;



namespace TwitterApi.DTO
{
    public class FollowRequestDTO
    {
        [Required]
        public string userId { get; set; } = null!;
    }
}