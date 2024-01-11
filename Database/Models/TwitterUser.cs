using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace TwitterApi.Models
{
    public class TwitterUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = null!;
        [MaxLength(255)]
        public string Bio { get; set; } = null!;
        [MaxLength(50)]
        public string Location { get; set; } = null!;
        public bool IsVerified { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int TweetsCount { get; set; }
        [JsonIgnore]
        public ICollection<Tweet> Tweets { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Follow> Followers { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Follow> Following { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Like> Likes { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Retweet> Retweets { get; set; } = null!;
        [JsonIgnore]
        public ICollection<UserChatroom> UserChatrooms { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Message> SentMessages { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Message> ReceivedMessages { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; } = null!;
    }
}