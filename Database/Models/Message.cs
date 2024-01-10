using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TwitterApi.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public TwitterUser Sender { get; set; } = null!;
        public string RecipientId { get; set; } = string.Empty;
        public TwitterUser Recipient { get; set; } = null!;
        public int ChatroomId { get; set; }
        public ChatRoom Chatroom { get; set; } = null!;
    }
}