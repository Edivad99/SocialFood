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
    private readonly ILogger<AuthController> logger;

    public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
    {
        this.identityService = identityService;
        this.logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await identityService.LoginAsync(request);
        if (response != null)
            return Ok(response);
        return BadRequest();
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    public async Task<IActionResult> Signin(LoginRequest request)
    {
        var response = await identityService.SigninAsync(request);
        if (response != null)
            return Ok(response);
        return BadRequest();
    }
}

