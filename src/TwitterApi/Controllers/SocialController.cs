using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TwitterApi.Models;
using TwitterApi.DTO;
using TwitterApi.Services.NotificationService;
using TwitterApi.Services.SocialService;
using TwitterApi.Database;
using Microsoft.EntityFrameworkCore;


namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SocialController(
        ISocialService socialService,
        INotificationService notificationService,
        UserManager<TwitterUser> userManager,
        ILogger<SocialController> logger,
        DataContext context
        ) : ControllerBase
    {
        private readonly ISocialService _socialService = socialService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly UserManager<TwitterUser> _userManager = userManager;
        private readonly ILogger<SocialController> _logger = logger;
        private readonly DataContext _context = context;

        [HttpPost]
        public async Task<IActionResult> FollowUser(FollowRequestDTO input)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userToFollow = await _userManager.FindByIdAsync(input.userId);

            if (user == null || userToFollow == null)
            {
                return NotFound("User not found");
            }

            var existing = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == user.Id && f.FollowingId == userToFollow.Id);
            if (existing != null)
            {
                return StatusCode(400, new 
                {
                    Message = "Already following user"
                });
            }

            try {
                await _socialService.FollowUserAsync(user, userToFollow);
                return StatusCode(201, new 
                {
                    Message = $"{userToFollow.UserName} followed successfully"
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error following user");
                return StatusCode(500, "Error following user");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UnfollowUser(FollowRequestDTO input)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userToUnfollow = await _userManager.FindByIdAsync(input.userId);

            if (user == null || userToUnfollow == null)
            {
                return NotFound("User not found");
            }

            var existing = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == user.Id && f.FollowingId == userToUnfollow.Id);

            if (existing == null)
            {
                return StatusCode(400, new 
                {
                    Message = "Not following user"
                });
            }

            try {
                await _socialService.UnFollowUserAsync(user, userToUnfollow, existing);
                return StatusCode(201, new 
                {
                    Message = $"{userToUnfollow.UserName} unfollowed successfully"
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error unfollowing user");
                return StatusCode(500, "Error unfollowing user");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikeTweet(LikeRequestDTO input)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var tweet = await _context.Tweets.FirstOrDefaultAsync(t => t.Id == input.tweetId);
            if (tweet == null)
            {
                return NotFound("Tweet not found");
            }

            var existing = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == user.Id && l.TweetId == tweet.Id);
            if (existing != null)
            {
                return StatusCode(400, new 
                {
                    Message = "Already liked tweet"
                });
            }

            try {
                await _socialService.LikeTweetAsync(user, input, tweet);
                return StatusCode(201, new 
                {
                    Message = $"Tweet liked successfully"
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error liking tweet");
                return StatusCode(500, "Error liking tweet");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Retweet(RetweetRequestDTO input)
        {
            if (User.Identity == null || User.Identity.Name == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var tweet = await _context.Tweets
                .FirstOrDefaultAsync(t => t.Id == input.tweetId);

            if (tweet == null)
            {
                return NotFound("Tweet not found");
            }
        
            var existing = await _context.Retweets
                .FirstOrDefaultAsync(r => r.RetweeterId == user.Id && r.TweetId == tweet.Id);
            if (existing != null)
            {
                return StatusCode(400, new 
                {
                    Message = "Already retweeted tweet"
                });
            }

            try {
                await _socialService.RetweetAsync(user, input, tweet);
                return StatusCode(201, new 
                {
                    Message = $"Tweet retweeted successfully"
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error retweeting tweet");
                return StatusCode(500, "Error retweeting tweet");
            }
        }
    }
}