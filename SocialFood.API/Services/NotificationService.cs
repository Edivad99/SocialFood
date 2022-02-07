using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SocialFood.API.Extensions;
using SocialFood.API.Models;
using SocialFood.API.Settings;
using SocialFood.Data.Entity;
using SocialFood.Data.Repository;
using SocialFood.Shared.Models;
using WebPush;

namespace SocialFood.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository notificationRepository;
    private readonly NotificationSettings settings;
    private readonly ILogger<NotificationService> logger;

    public NotificationService(INotificationRepository notificationRepository, IOptions<NotificationSettings> notificationSettings, ILogger<NotificationService> logger)
    {
        this.notificationRepository = notificationRepository;
        this.settings = notificationSettings.Value;
        this.logger = logger;
    }

    public async Task<Response<NotificationSubscription>> SubscribeUserAsync(Guid IDUser, NotificationSubscription notification)
    {
        try
        {
            logger.LogInformation($"New SubscribeUserAsync request: {IDUser}");

            var notificationDb = new Notification()
            {
                IDUser = IDUser.ToString(),
                Auth = notification.Auth,
                P256dh = notification.P256dh,
                Url = notification.Url
            };

            //await notificationRepository.RemoveNotificationSubscriptionByIdAsync(notificationDb.IDUser);
            await notificationRepository.AddNotificationSubscriptionAsync(notificationDb);

            var response = new Response<NotificationSubscription>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = notification
            };

            logger.LogInformation($"SubscribeUserAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in SubscribeUserAsync with userid: {IDUser}");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = null };
        }
    }

    public Response<string> GetPublicKey()
    {
        try
        {
            logger.LogInformation("New GetPublicKey request");
            var response = new Response<string>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = settings.PublicKey
            };

            logger.LogInformation($"GetPublicKey response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "New error in GetPublicKey");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = null };
        }
    }

    public async Task NotificationNewFriendship(Guid friend, string currentUser)
    {
        logger.LogInformation("Sending notification about new friendship");
        var notificationSubscriptions = await notificationRepository.GetNotificationSubscriptionAsync(friend.ToString());
        var subscriptionsTask = notificationSubscriptions.Select(x => new NotificationSubscription()
        {
            Auth = x.Auth,
            P256dh = x.P256dh,
            Url = x.Url
        }).Select(x => SendNotificationAsync(x, $"{currentUser} ti ha aggiunto ai suoi amici", $"profile?username={currentUser}"));

        await Task.WhenAll(subscriptionsTask);
    }

    public async Task NotificationNewPhoto(Guid userID, string username)
    {
        logger.LogInformation("Sending notification about new photo");
        var followerSubscription = await notificationRepository.GetFollowerNotificationSubscriptionAsync(userID.ToString());
        var subscriptionsTask = followerSubscription.Select(x => new NotificationSubscription()
        {
            Auth = x.Auth,
            P256dh = x.P256dh,
            Url = x.Url
        }).Select(x => SendNotificationAsync(x, $"{username} ha pubblicato una nuova foto", $"profile?username={username}"));

        await Task.WhenAll(subscriptionsTask);
    }

    private async Task SendNotificationAsync(NotificationSubscription subscription, string message, string url)
    {
        //https://vapidkeys.com/
        var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
        var vapidDetails = new VapidDetails(settings.Mail, settings.PublicKey, settings.PrivateKey);
        var webPushClient = new WebPushClient();
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                message,
                url
            });
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
        }
        catch (WebPushException e)
        {
            logger.LogError(e, $"Error sending push notification: {e.Message}");
            if(e.Message == "Subscription no longer valid" || e.HttpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                await notificationRepository.RemoveNotificationSubscriptionByUrlAsync(e.PushSubscription.Endpoint);
                logger.LogInformation(e, "Removed notification subscription by url");
            }
        }
        catch(Exception e)
        {
            logger.LogError(e, $"Error sending push notification: {e.Message}");
        }
    }
}

