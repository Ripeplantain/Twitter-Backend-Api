using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TwitterApi.Models
{
    [Table("Tweets")]
    public class Tweet
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(280)]
        public string Content { get; set; } = null!;

        public int LikesCount { get; set; }

        public int RetweetsCount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public TwitterUser User { get; set; } = null!;
    }
}