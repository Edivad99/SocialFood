using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialFood.API.Services;
using SocialFood.Shared.Models;

namespace SocialFood.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService identityService;

    public AuthController(IIdentityService identityService)
    {
        this.identityService = identityService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await identityService.LoginAsync(request);
        return StatusCode(response.StatusCode, response.Result);
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registration(RegistrationRequest request)
    {
        var response = await identityService.RegistrationAsync(request);
        return StatusCode(response.StatusCode, response.Result);
    }
}

