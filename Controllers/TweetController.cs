using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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
        private readonly IDistributedCache _cache;

        public TweetController(
            DataContext context, 
            ILogger<TweetController> logger, 
            UserManager<TwitterUser> userManager,
            IDistributedCache cache
        )
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _cache = cache;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO>> GetTweets(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 25,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var cacheKey = $"tweets-{pageIndex}-{pageSize}";
                var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (string.IsNullOrEmpty(cachedResponse))
                {
                    var tweets = await _context.Tweets
                        .Include(t => t.User)
                        .OrderByDescending(t => t.CreatedAt)
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    if (tweets.Count == 0)
                    {
                        return NotFound(new ResponseDTO
                        {
                            Message = "No tweets found",
                            Count = 0,
                            Tweets = []
                        });
                    }
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        MaxDepth = 1
                    };
                    await _cache.SetStringAsync(
                        cacheKey,
                        JsonConvert.SerializeObject(tweets),
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
                        },
                        cancellationToken
                    );

                    return Ok(new ResponseDTO
                    {
                        Message = "Tweets retrieved successfully",
                        Count = tweets.Count,
                        Tweets = tweets.Select(t => new TweetDTO
                        {
                            Id = t.Id,
                            Content = t.Content,
                            CreatedAt = t.CreatedAt,
                            User = new UserDTO
                            {
                                Id = t.User.Id,
                                UserName = t.User.UserName ?? string.Empty,
                                Email = t.User.Email ?? string.Empty,
                                FullName = t.User.FullName
                            }
                        }).ToList(),
                    });
                } else {
                    var tweets = JsonConvert.DeserializeObject<List<Tweet>>(cachedResponse);
                    if (tweets == null)
                    {
                        return NotFound(new ResponseDTO
                        {
                            Message = "No tweets found",
                            Count = 0,
                            Tweets = []
                        });
                    }
                    return Ok(new ResponseDTO
                    {
                        Message = "Tweets retrieved successfully",
                        Count = tweets.Count,
                        Tweets = tweets.Select(t => new TweetDTO
                        {
                            Id = t.Id,
                            Content = t.Content,
                            CreatedAt = t.CreatedAt,
                            User = new UserDTO
                            {
                                Id = t.User.Id,
                                UserName = t.User.UserName ?? string.Empty,
                                Email = t.User.Email ?? string.Empty,
                                FullName = t.User.FullName
                            }
                        }).ToList(),
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while processing the request");
                return StatusCode(500, new ResponseDTO
                {
                    Message = "An error occurred while retrieving tweets",
                    Count = 0,
                    Tweets = []
                });
            }
        }

        [HttpGet("{username}/tweets")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO>> GetUserTweets(string username)
        {
            try {
                var user = await _context.Users
                    .Include(u => u.Tweets)
                    .FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null) {
                    return NotFound();
                }
                return Ok(new ResponseDTO {
                    Message = "Tweets retrieved successfully",
                    Count = user.Tweets.Count,
                    Tweets = user.Tweets.Select(t => new TweetDTO {
                        Id = t.Id,
                        Content = t.Content,
                        User = new UserDTO {
                            Id = t.User.Id,
                            UserName = t.User.UserName ?? string.Empty,
                            Email = t.User.Email ?? string.Empty,
                            FullName = t.User.FullName                        
                        },
                        CreatedAt = t.CreatedAt
                    }).ToList(),
                });
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO>> GetTweet(int id)
        {
            try {
                var tweet = await _context.Tweets
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (tweet == null || tweet.User == null 
                    || tweet.User.UserName == null 
                    || tweet.User.Email == null) {
                    return NotFound();
                }
                return Ok(new ResponseDTO {
                    Message = "Tweet retrieved successfully",
                    Count = 1,
                    Tweets = new List<TweetDTO> {
                        new TweetDTO {
                            Id = tweet.Id,
                            Content = tweet.Content,
                            User = new UserDTO {
                                Id = tweet.User.Id,
                                UserName = tweet.User.UserName,
                                Email = tweet.User.Email,
                                FullName = tweet.User.FullName
                            },
                            CreatedAt = tweet.CreatedAt
                        }
                    },
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
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (ModelState.IsValid)
                {
                    if (User.Identity == null || User.Identity.Name == null)
                    {
                        return Unauthorized();
                    }
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);

                    if (user == null)
                    {
                        return Unauthorized("User not found");
                    }

                    var newTweet = new Tweet
                    {
                        Content = input.Content,
                        LikesCount = 0,
                        RetweetsCount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UserId = user.Id,
                        User = user
                    };

                    await _context.Tweets.AddAsync(newTweet);
                    await _context.SaveChangesAsync();

                    // Increment the TweetsCount for the user
                    user.TweetsCount++;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return StatusCode(201, new
                    {
                        Message = "Tweet created successfully",
                    });
                }

                await transaction.RollbackAsync();
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
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