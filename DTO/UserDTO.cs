using System.ComponentModel.DataAnnotations;


namespace TwitterApi.DTO
{
    public class UserDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = null!;

        [MaxLength(255)]
        public string Bio { get; set; } = null!;

        [MaxLength(50)]
        public string Location { get; set; } = null!;

        public bool IsVerified { get; set; }

        public int FollowersCount { get; set; }

        public int FollowingCount { get; set; }

        public int TweetsCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}