using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using TwitterApi.Database;
using TwitterApi.DTO;
using Newtonsoft.Json;
using TwitterApi.Models;
using Microsoft.AspNetCore.Identity;



namespace TwitterApi.Services.TweetService
{
    public class TweetService(
        DataContext context, IDistributedCache cache, ILogger<TweetService> logger,
        UserManager<TwitterUser> userManager
        ) : ITweetService
    {
        private readonly DataContext _context = context;
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<TweetService> _logger = logger;
        private readonly UserManager<TwitterUser> _userManager = userManager;


        public async Task<List<TweetDTO>> GetTweetAsync(int pageSize, int pageIndex, CancellationToken cancellationToken)
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
                    .ToListAsync(cancellationToken: cancellationToken);

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

                return tweets.Select(t => new TweetDTO
                {
                    Id = t.Id,
                    Content = t.Content,
                    User = new UserDTO
                    {
                        Id = t.User?.Id ?? string.Empty,
                        FullName = t.User?.FullName ?? string.Empty,
                        UserName = t.User?.UserName ?? string.Empty,
                        Bio = t.User?.Bio ?? string.Empty,
                        Location = t.User?.Location ?? string.Empty,
                        Email = t.User?.Email ?? string.Empty,
                    },
                    CreatedAt = t.CreatedAt
                }).ToList();

            } else {
                var tweets = JsonConvert.DeserializeObject<List<Tweet>>(cachedResponse);
                if (tweets == null)
                {
                    return [];
                }
                return tweets.Select(t => new TweetDTO
                {
                    Id = t.Id,
                    Content = t.Content,
                    User = new UserDTO
                    {
                        Id = t.User?.Id ?? string.Empty,
                        FullName = t.User?.FullName ?? string.Empty,
                        UserName = t.User?.UserName ?? string.Empty,
                        Bio = t.User?.Bio ?? string.Empty,
                        Location = t.User?.Location ?? string.Empty,
                        Email = t.User?.Email ?? string.Empty,
                    },
                    CreatedAt = t.CreatedAt
                }).ToList();
            }
        }

        public async Task<List<TweetDTO>?> GetUserTweetAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Tweets)
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return null;
            }
            return user.Tweets.Select(t => new TweetDTO
            {
                Id = t.Id,
                Content = t.Content,
                User = new UserDTO
                {
                    Id = t.User?.Id ?? string.Empty,
                    FullName = t.User?.FullName ?? string.Empty,
                    UserName = t.User?.UserName ?? string.Empty,
                    Bio = t.User?.Bio ?? string.Empty,
                    Location = t.User?.Location ?? string.Empty,
                    Email = t.User?.Email ?? string.Empty,
                },
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public async Task<TweetDTO?> GetTweetByIdAsync(int id)
        {
            var tweet = await _context.Tweets
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tweet == null || tweet.User == null
                || tweet.User.UserName == null
                || tweet.User.Email == null)
            {
                return null;
            }
            return new TweetDTO
            {
                Id = tweet.Id,
                Content = tweet.Content,
                User = new UserDTO
                {
                    Id = tweet.User.Id,
                    UserName = tweet.User.UserName,
                    Email = tweet.User.Email,
                    FullName = tweet.User.FullName,
                    Bio = tweet.User.Bio,
                    Location = tweet.User.Location
                },
                CreatedAt = tweet.CreatedAt
            };
        }

        public async Task<TweetDTO> CreateTweetAsync(string username, TweetDTO input)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("User has no username");

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

                user.TweetsCount++;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new TweetDTO
                {
                    Id = newTweet.Id,
                    Content = newTweet.Content,
                    User = new UserDTO
                    {
                        Id = newTweet.User?.Id ?? string.Empty,
                        FullName = newTweet.User?.FullName ?? string.Empty,
                        UserName = newTweet.User?.UserName ?? string.Empty,
                        Bio = newTweet.User?.Bio ?? string.Empty,
                        Location = newTweet.User?.Location ?? string.Empty,
                        Email = newTweet.User?.Email ?? string.Empty,
                    },
                    CreatedAt = newTweet.CreatedAt
                };
            } catch (Exception e) {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Error occurred while processing the request");
                throw new Exception("An error occurred while creating tweet");
            }
        }

        public async Task<TweetDTO?> UpdateTweetAsync(int id, TweetDTO input)
        {
            try {
                var tweet = await _context.Tweets.FindAsync(id);
                _logger.LogInformation("Tweet: {0}", tweet!.Content);
                if (tweet == null)
                {
                    return null;
                }
                tweet.Content = input.Content;
                await _context.SaveChangesAsync();
                return new TweetDTO
                {
                    Id = tweet.Id,
                    Content = tweet.Content,
                    // User = new UserDTO
                    // {
                    //     Id = tweet.User?.Id ?? string.Empty,
                    //     FullName = tweet.User?.FullName ?? string.Empty,
                    //     UserName = tweet.User?.UserName ?? string.Empty,
                    //     Bio = tweet.User?.Bio ?? string.Empty,
                    //     Location = tweet.User?.Location ?? string.Empty,
                    //     Email = tweet.User?.Email ?? string.Empty,
                    // },
                    CreatedAt = tweet.CreatedAt
                };
            } catch (Exception e) {
                _logger.LogError(e, "Error occurred while processing the request");
                throw new Exception("An error occurred while updating tweet");
            }
        }

        public async Task<bool> DeleteTweetAsync(int id)
        {
            try {
                var tweet = await _context.Tweets.FindAsync(id);
                if (tweet == null)
                {
                    return false;
                }
                _context.Tweets.Remove(tweet);
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception e) {
                _logger.LogError(e, "Error occurred while processing the request");
                throw new Exception("An error occurred while deleting tweet");
            }
        }
    }
}