

namespace TwitterApi.Models
{
    public class UserChatroom
    {
        public string UserId { get; set; } = string.Empty;
        public TwitterUser User { get; set; } = null!;

        public int ChatroomId { get; set; }
        public ChatRoom Chatroom { get; set; } = null!;
    }
}