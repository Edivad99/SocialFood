using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public interface INotificationRepository
{
    Task AddNotificationSubscriptionAsync(Notification notification);
    Task<IEnumerable<Notification>> GetFollowerNotificationSubscriptionAsync(string IDUser);
    Task<IEnumerable<Notification>> GetNotificationSubscriptionAsync(string IDUser);
    Task RemoveNotificationSubscriptionByIdAsync(string IDUser);
    Task RemoveNotificationSubscriptionByUrlAsync(string url);
}
