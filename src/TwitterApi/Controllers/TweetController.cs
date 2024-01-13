using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using TwitterApi.Models;
using TwitterApi.DTO;
using TwitterApi.Services.TweetService;


namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class TweetController(
        ILogger<TweetController> logger,
        UserManager<TwitterUser> userManager,
        ITweetService tweetService
        ) : ControllerBase
    {
        private readonly ILogger<TweetController> _logger = logger;
        private readonly UserManager<TwitterUser> _userManager = userManager;
        private readonly ITweetService _tweetService = tweetService;

        [HttpGet]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO<List<Tweet>>>> GetTweets(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 25,
            CancellationToken cancellationToken = default
        )
        {
            try {
                var tweets = await _tweetService.GetTweetAsync(pageSize, pageIndex, cancellationToken);
                return Ok(new ResponseDTO<List<Tweet>>
                {
                    Message = "Tweets retrieved successfully",
                    Count = tweets.Count,
                    Tweets = tweets
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error occurred while processing the request");
                return StatusCode(500, new ResponseDTO<List<Tweet>>
                {
                    Message = "An error occurred while retrieving tweets",
                    Count = 0,
                    Tweets = []
                });
            }
        }

        [HttpGet("{username}/tweets")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<ResponseDTO<List<Tweet>>>> GetUserTweets(string username)
        {
            try {
                var tweets = await _tweetService.GetUserTweetAsync(username);
                if (tweets == null) {
                    return NotFound(new ResponseDTO<List<Tweet>>
                    {
                        Message = "User not found",
                        Count = 0,
                        Tweets = []
                    });
                }
                return Ok(new ResponseDTO<List<Tweet>>
                {
                    Message = "Tweets retrieved successfully",
                    Count = tweets.Count,
                    Tweets = tweets
                });
            } catch (Exception e) {
                _logger.LogError(e, "Error occurred while processing the request");
                return StatusCode(500, new ResponseDTO<List<Tweet>>
                {
                    Message = "An error occurred while retrieving tweets",
                    Count = 0,
                    Tweets = []
                });
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult<TweetDTO>> GetTweet(int id)
        {
            try {
                var tweet = await _tweetService.GetTweetByIdAsync(id);
                if (tweet == null)
                {
                    return NotFound("Tweet not found");
                } else {
                    return Ok(tweet);
                }
            } catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while processing the request");
                return StatusCode(500, new ResponseDTO<List<Tweet>>
                {
                    Message = "An error occurred while retrieving tweets",
                    Count = 0,
                    Tweets = []
                });
            }
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> CreateTweet([FromBody] TweetDTO input)
        {
            try {
                if (ModelState.IsValid) {
                    var username = User.Identity?.Name ?? throw new Exception("User not found");
                    var tweet = await _tweetService.CreateTweetAsync(username, input);
                    return Ok(tweet);
                } else {
                    return BadRequest(ModelState);
                }
            } catch (Exception e) {
                _logger.LogError(e, "Error occurred while processing the request");
                return StatusCode(500, new ResponseDTO<List<Tweet>>
                {
                    Message = "An error occurred while retrieving tweets",
                    Count = 0,
                    Tweets = []
                });
            }
        }


        [HttpPut("{id}")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> UpdateTweet(int id, TweetDTO input)
        {
            try {
                if (ModelState.IsValid) {
                    var tweet = await _tweetService.UpdateTweetAsync(id, input);
                    if (tweet == null) {
                        return NotFound();
                    } else {
                        return Ok(tweet);
                    }
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
                var tweet = await _tweetService.DeleteTweetAsync(id);
                if (tweet) {
                    return Ok();
                } else {
                    return NotFound("Tweet not found");
                }
            } catch (Exception e) {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }
    }
}