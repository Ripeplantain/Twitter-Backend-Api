using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TwitterApi.Models;
using TwitterApi.Database;
using TwitterApi.DTO;



namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class TweetController: ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<TweetController> _logger;
        private readonly UserManager<TwitterUser> _userManager;

        public TweetController(DataContext context, ILogger<TweetController> logger, UserManager<TwitterUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> GetTweets()
        {
            try {
                var tweets = await _context.Tweets
                    .Include(t => t.User)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
                return Ok(tweets);
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{username}/tweets")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> GetUserTweets(string username)
        {
            try {
                var user = await _context.Users
                    .Include(u => u.Tweets)
                    .FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null) {
                    return NotFound();
                }
                return Ok(user.Tweets);
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> GetTweet(int id)
        {
            try {
                var tweet = await _context.Tweets
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (tweet == null) {
                    return NotFound();
                }
                return Ok(tweet);
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> CreateTweet(TweetDTO input)
        {
            try {
                if (ModelState.IsValid) {
                    var user = await _userManager.GetUserAsync(User);
                    if (input != null && user != null) {
                        var newTweet = new Tweet {
                        Content = input.Content,
                        LikesCount = 0,
                        RetweetsCount = 0,
                        CreatedAt = DateTime.Now,
                        UserId = user.Id,
                        User = user
                        };
                    await _context.Tweets.AddAsync(newTweet);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, newTweet);
                    } else {
                        return BadRequest();
                    }
                } else {
                    return BadRequest(ModelState);
                }
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> UpdateTweet(int id, TweetDTO input)
        {
            try {
                if (ModelState.IsValid) {
                    var tweet = await _context.Tweets.FindAsync(id);
                    if (tweet == null) {
                        return NotFound();
                    }
                    tweet.Content = input.Content;
                    await _context.SaveChangesAsync();
                    return Ok(tweet);
                } else {
                    return BadRequest(ModelState);
                }
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> DeleteTweet(int id)
        {
            try {
                var tweet = await _context.Tweets.FindAsync(id);
                if (tweet == null) {
                    return NotFound();
                }
                _context.Tweets.Remove(tweet);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }
    }
}