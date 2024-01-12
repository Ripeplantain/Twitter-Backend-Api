using TwitterApi.Models;


namespace TwitterApi.DTO
{
    public class UserResponseDTO
    {
        public UserDTO User { get; set; } = null!;
        public ICollection<Tweet> Tweets { get; set; } = null!;
    }   
}