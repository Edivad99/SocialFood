﻿using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface INotificationService
{
    Response<string> GetPublicKey();
    Task NotificationNewFriendship(Guid friend, string currentUser);
    Task<Response<NotificationSubscription>> SubscribeUserAsync(Guid IDUser, NotificationSubscription notification);
}
