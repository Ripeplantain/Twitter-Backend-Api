using TwitterApi.Models;



namespace TwitterApi.Services.NotificationService
{
    public interface INotificationService
    {
        void Notify(string userId, string type, string message);
        Task<List<Notification>> FetchNotifications(string userId);
    }
}