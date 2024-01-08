using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


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

        public ICollection<Tweet> Tweets { get; set; } = null!;
    }
}