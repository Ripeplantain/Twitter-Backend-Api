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
        public async Task<ActionResult<ResponseDTO<List<Tweet>>>> GetTweets()
        {
            try {
                var tweets = await _context.Tweets
                    .Include(t => t.User)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
                return Ok(new ResponseDTO<List<Tweet>> {
                    Message = "Tweets retrieved successfully",
                    Count = tweets.Count,
                    Data = tweets
                });
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{username}/tweets")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO<List<Tweet>>>> GetUserTweets(string username)
        {
            try {
                var user = await _context.Users
                    .Include(u => u.Tweets)
                    .FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null) {
                    return NotFound();
                }
                return Ok(new ResponseDTO<List<Tweet>> {
                    Message = "Tweets retrieved successfully",
                    Count = user.Tweets.Count,
                    Data = user.Tweets.ToList()
                });
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO<Tweet>>> GetTweet(int id)
        {
            try {
                var tweet = await _context.Tweets
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (tweet == null) {
                    return NotFound();
                }
                return Ok(new ResponseDTO<Tweet> {
                    Message = "Tweet retrieved successfully",
                    Count = 1,
                    Data = tweet
                });
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
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user == null) {
                        return Unauthorized("User not found");
                    }
                    var newTweet = new Tweet {
                        Content = input.Content,
                        LikesCount = 0,
                        RetweetsCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UserId = user.Id,
                        User = user
                    };
                    await _context.Tweets.AddAsync(newTweet);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, new {
                        Message = "Tweet created successfully",
                    });
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
                return Ok(new {
                    Message = "Tweet deleted successfully"
                });
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }
    }
}