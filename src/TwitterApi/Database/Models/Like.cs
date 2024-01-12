using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace TwitterApi.Models
{
    [Table("Likes")]
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TweetId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public Tweet Tweet { get; set; } = null!;

        public TwitterUser User { get; set; } = null!;
    }
}