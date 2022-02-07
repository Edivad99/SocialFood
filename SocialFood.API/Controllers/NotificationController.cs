using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialFood.API.Services;
using SocialFood.Shared.Models;
using SocialFood.Shared.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SocialFood.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService notificationService;

    public NotificationController(INotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    [HttpGet("publickey")]
    [AllowAnonymous]
    public IActionResult GetPublicKey()
    {
        var response = notificationService.GetPublicKey();
        return StatusCode(response.StatusCode, response.Result);
    }

    [HttpPut("subscribe")]
    public async Task<IActionResult> Subscribe(NotificationSubscription subscription)
    {
        var response = await notificationService.SubscribeUserAsync(User.GetId(), subscription);
        return StatusCode(response.StatusCode, response.Result);
    }
}
