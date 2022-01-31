using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialFood.API.Services;
using SocialFood.Shared.Models;

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
        var response = await accountService.GetUserFromUsernameAsync(username);
        if (!response.Any())
            return NotFound(response);
        return Ok(response.Select(x => new UserDTO { Username = x.Username }));
    }
}
