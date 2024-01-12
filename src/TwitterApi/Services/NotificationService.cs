using Microsoft.AspNetCore.SignalR;
using TwitterApi.Infrastructure;
using TwitterApi.Database;
using TwitterApi.Models;
using Microsoft.EntityFrameworkCore;


namespace TwitterApi.Services
{
    public class NotificationService(IHubContext<SignalServer> hubContext, DataContext dataContext)
    {
        private readonly IHubContext<SignalServer> _hubContext = hubContext;
        private readonly DataContext _dataContext = dataContext;

        public void Notify(string userId, string type, string message)
        {
            Notification notification = new()
            {
                Type = type,
                Message = message,
                UserId = userId
            };

            _dataContext.Notifications.Add(notification);
            _dataContext.SaveChanges();

            _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task<List<Notification>> FetchNotifications(string userId)
        {
            var notifications = await _dataContext.Notifications.Where(n => n.UserId == userId).ToListAsync();
            return notifications;
        }
    }
}