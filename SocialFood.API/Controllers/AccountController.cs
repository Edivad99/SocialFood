using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialFood.API.Services;
using SocialFood.Shared.Models;
using SocialFood.Shared.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SocialFood.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly ILogger<AccountController> logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        this.accountService = accountService;
        this.logger = logger;
    }

    [HttpGet("finduser/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserFromUsernameAsync(string username)
    {
        var response = await accountService.GetUsersFromUsernameAsync(username);
        if (!response.Any())
            return NotFound(response);
        return Ok(response);
    }

    [HttpGet("me/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYourFriends() => await GetUsersFriends(User.GetUsername()!);

    [HttpGet("{username}/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersFriends(string username)
    {
        var response = await accountService.GetUsersFriendsAsync(username);
        return Ok(response);
    }

    [HttpPut("me/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFriend(string friendUsername)
    {
        var response = await accountService.AddFriendAsync(User.GetId(), friendUsername);
        return response ? Ok() : BadRequest();
    }

    [HttpDelete("me/friends/{friendUsername}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriend(string friendUsername)
    {
        var response = await accountService.RemoveFriendAsync(User.GetId(), friendUsername);
        return response ? Ok() : BadRequest();
    }
}
