using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TwitterApi.Models;
using TwitterApi.Database;
using TwitterApi.DTO;


namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SocialController : ControllerBase
    {
        private readonly UserManager<TwitterUser> _userManager;
        private readonly ILogger<SocialController> _logger;
        private readonly DataContext _context;

        public SocialController(
            UserManager<TwitterUser> userManager, 
            ILogger<SocialController> logger,
            DataContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser(FollowRequestDTO input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userToFollow = await _userManager.FindByIdAsync(input.userId);

                if (user == null || userToFollow == null)
                {
                    return NotFound();
                }

                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == user.Id && f.FollowingId == userToFollow.Id);

                if (existingFollow != null)
                {
                    return BadRequest("Already following user");
                }

                var newFollow = new Follow
                {
                    FollowerId = user.Id,
                    FollowingId = userToFollow.Id,
                    CreatedAt = DateTime.UtcNow,
                    Follower = user,
                    Following = userToFollow
                };

                await _context.Follows.AddAsync(newFollow);
                await _context.SaveChangesAsync();

                user.FollowingCount++;
                userToFollow.FollowersCount++;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return StatusCode(201, new 
                {
                    Message = $"{userToFollow.UserName} followed successfully"
                });

            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error following user");
                await transaction.RollbackAsync();
                return StatusCode(500, "Error following user");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UnfollowUser(FollowRequestDTO input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userToUnfollow = await _userManager.FindByIdAsync(input.userId);

                if (user == null || userToUnfollow == null)
                {
                    return NotFound();
                }

                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == user.Id && f.FollowingId == userToUnfollow.Id);

                if (existingFollow == null)
                {
                    return BadRequest("Not following user");
                }

                _context.Follows.Remove(existingFollow);
                await _context.SaveChangesAsync();

                user.FollowingCount--;
                userToUnfollow.FollowersCount--;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return StatusCode(200, new 
                {
                    Message = $"{userToUnfollow.UserName} unfollowed successfully"
                });

            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfollowing user");
                await transaction.RollbackAsync();
                return StatusCode(500, "Error unfollowing user");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikeTweet(LikeRequestDTO input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var tweet = await _context.Tweets
                    .FirstOrDefaultAsync(t => t.Id == input.tweetId);

                if (user == null || tweet == null)
                {
                    return NotFound();
                }

                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.TweetId == tweet.Id && l.UserId == user.Id);

                if (existingLike != null)
                {
                    return BadRequest("Already liked tweet");
                }

                var newLike = new Like
                {
                    TweetId = tweet.Id,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Tweet = tweet,
                    User = user
                };

                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();

                tweet.LikesCount++;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return StatusCode(201, new 
                {
                    Message = $"Tweet liked successfully"
                });

            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking tweet");
                await transaction.RollbackAsync();
                return StatusCode(500, "Error liking tweet");
            }
        }
    }
}