using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TwitterApi.Models
{
    [Table("Follows")]
    public class Follow
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string FollowerId { get; set; } = null!;

        [Required]
        public string FollowingId { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public TwitterUser Follower { get; set; } = null!;

        [Required]
        public TwitterUser Following { get; set; } = null!;
    }
}
