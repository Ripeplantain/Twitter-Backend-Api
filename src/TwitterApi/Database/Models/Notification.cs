using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TwitterApi.Models
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public TwitterUser User { get; set; } = null!;
    }
}