using TwitterApi.DTO;
using TwitterApi.Models;



namespace TwitterApi.Services.SocialService
{
    public interface ISocialService
    {
        Task FollowUserAsync(TwitterUser user, TwitterUser userToFollow);
        Task UnFollowUserAsync(TwitterUser user, TwitterUser userToUnFollow, Follow existing);
        Task LikeTweetAsync(TwitterUser user, LikeRequestDTO input, Tweet tweet);
        Task RetweetAsync(TwitterUser user, RetweetRequestDTO input, Tweet tweet);
    }
}