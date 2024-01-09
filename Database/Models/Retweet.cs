using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TwitterApi.Models
{
    [Table("Retweets")]
    public class Retweet
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public string Caption { get; set; } = string.Empty;

        [Required]
        public string RetweeterId { get; set; } = null!;

        [Required]
        public int TweetId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public TwitterUser Retweeter { get; set; } = null!;

        [Required]
        public Tweet Tweet { get; set; } = null!;
    }
}