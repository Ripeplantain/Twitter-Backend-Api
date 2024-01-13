using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TwitterApi.Database;
using TwitterApi.DTO;
using TwitterApi.Models;



namespace TwitterApi.Services.SocialService
{
    public class SocialService(
        DataContext context,
        UserManager<TwitterUser> userManager,
        ILogger<SocialService> logger
        ) : ISocialService
    {
        private readonly DataContext _context = context;
        private readonly UserManager<TwitterUser> _userManager = userManager;
        private readonly ILogger<SocialService> _logger = logger;

        public async Task FollowUserAsync(TwitterUser user, TwitterUser userToFollow)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                var newFollow = new Follow 
                {
                    FollowerId = user.Id,
                    FollowingId = userToFollow.Id,
                    Follower = user,
                    Following = userToFollow
                };
                await _context.Follows.AddAsync(newFollow);
                await _context.SaveChangesAsync();

                user.FollowersCount++;
                userToFollow.FollowingCount++;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            } catch (Exception e) {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error following user");
            }
        }

        public async Task UnFollowUserAsync(TwitterUser user, TwitterUser userToUnFollow, Follow existing)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {

                _context.Follows.Remove(existing);
                await _context.SaveChangesAsync();

                user.FollowersCount--;
                userToUnFollow.FollowingCount--;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch (Exception e) {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error unfollowing user");
            }
        }

        public async Task LikeTweetAsync(TwitterUser user, LikeRequestDTO input, Tweet tweet)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {

                var newLike = new Like 
                {
                    UserId = user.Id,
                    TweetId = tweet.Id,
                    User = user,
                    Tweet = tweet
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();

                tweet.LikesCount++;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch (Exception e) {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error liking tweet");
            }
        }

        public async Task RetweetAsync(TwitterUser user, RetweetRequestDTO input, Tweet tweet)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                var newRetweet = new Retweet 
                {
                    RetweeterId = user.Id,
                    TweetId = tweet.Id,
                    Retweeter = user,
                    Tweet = tweet
                };
                await _context.Retweets.AddAsync(newRetweet);
                await _context.SaveChangesAsync();

                tweet.RetweetsCount++;
                user.TweetsCount++;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch (Exception e) {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error retweeting tweet");
            }
        }
    }
}