using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;



namespace TwitterApi.Models
{
    [Table("ChatRoom")]
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<UserChatroom> UserChatrooms { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Message> Messages { get; set; } = null!;
    }
}