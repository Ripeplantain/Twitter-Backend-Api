using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace TwitterApi.Service
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToUser(string recipientId, string message)
        {
            // await Clients.User(userId).SendAsync("ReceiveMessage", message);
            var senderId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(senderId) && !string.IsNullOrEmpty(recipientId))
            {
                await Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message);
            } else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Error", "Invalid user");
            }
        }
    }
}