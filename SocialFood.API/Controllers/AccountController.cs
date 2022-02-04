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

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpGet("finduser/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserFromUsernameAsync(string username)
    {
        var response = await accountService.GetUsersFromUsernameAsync(username);
        return StatusCode(response.StatusCode, response.Result);
    }

    [HttpGet("me/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYourFriends() => await GetUsersFriends(User.GetUsername()!);

    [HttpGet("{username}/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersFriends(string username)
    {
        var response = await accountService.GetUsersFriendsAsync(username);
        return StatusCode(response.StatusCode, response.Result);
    }

    [HttpPut("me/friends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFriend(string friendUsername)
    {
        var response = await accountService.AddFriendAsync(User.GetId(), friendUsername);
        return StatusCode(response.StatusCode);
    }

    [HttpDelete("me/friends/{friendUsername}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriend(string friendUsername)
    {
        var response = await accountService.RemoveFriendAsync(User.GetId(), friendUsername);
        return StatusCode(response.StatusCode);
    }
}
