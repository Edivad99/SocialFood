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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await identityService.LoginAsync(request);
            if (response != null)
                return Ok(response);
            return BadRequest();
        } catch(Exception e)
        {
            return StatusCode(500, e.StackTrace);
        }
        
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registration(RegistrationRequest request)
    {
        var response = await identityService.RegistrationAsync(request);
        if (response != null)
            return Ok(response);
        return BadRequest();
    }
}

