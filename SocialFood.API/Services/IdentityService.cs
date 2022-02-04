using System;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SocialFood.Shared.Models;
using SocialFood.API.Settings;
using SocialFood.Data.Repository;
using SocialFood.Data.Entity;
using SocialFood.API.Models;

namespace SocialFood.API.Services;

public class IdentityService : IIdentityService
{
    private readonly JwtSettings jwtSettings;
    private readonly IAuthRepository authRepository;
    private readonly ILogger<IdentityService> logger;

    public IdentityService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettingsOptions, ILogger<IdentityService> logger)
    {
        this.authRepository = authRepository;
        this.logger = logger;
        this.jwtSettings = jwtSettingsOptions.Value;
    }

    private AuthResponse GenerateAuthResponse(User user)
    {
        //Nei claims puoi aggiungere tutte le informazioni utili che ritieni possano servirti
        //User id del db, username ecc.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.Firstname),
            new Claim(ClaimTypes.Surname, user.Lastname)
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expireDate = DateTime.UtcNow;
        var jwtSecurityToken = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims,
            expireDate, expireDate.AddDays(10), signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var response = new AuthResponse
        {
            AccessToken = accessToken,
            ExpireDate = Convert.ToInt32(expireDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)
        };
        return response;
    }

    public async Task<Response<AuthResponse?>> LoginAsync(LoginRequest request)
    {
        try
        {
            logger.LogInformation($"New login request: {request.Username}");
            var user = await authRepository.GetUserAsync(request.Username, request.Password);

            var response = new Response<AuthResponse?>()
            {
                StatusCode = user is null ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                Result = user is null ? null : GenerateAuthResponse(user)
            };
            logger.LogInformation($"Login response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in LoginAsync with LoginRequest: {request.Username}");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = null };
        }
    }

    public async Task<Response<AuthResponse>> RegistrationAsync(RegistrationRequest request)
    {
        try
        {
            logger.LogInformation($"New registration request: {request.Username}");

            var id = Guid.NewGuid().ToString();
            var user = new User(id, request.Username, request.Password, request.FirstName, request.LastName);
            await authRepository.InsertUserAsync(user);
            var response = new Response<AuthResponse>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = GenerateAuthResponse(user)
            };

            logger.LogInformation($"Registration response with status: {response.StatusCode}");
            return response;
        }
        catch(Exception e)
        {
            logger.LogError(e, $"New error in RegistrationAsync with RegistrationRequest: {request.Username}");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = null };
        }
    }
}
