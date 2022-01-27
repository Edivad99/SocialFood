using System;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SocialFood.Shared.Models;
using SocialFood.API.Settings;
using SocialFood.Data.Repository;

namespace SocialFood.API.Services;

public class IdentityService : IIdentityService
{
    private readonly JwtSettings jwtSettings;
    private readonly IAuthRepository authRepository;

    public IdentityService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettingsOptions)
    {
        this.authRepository = authRepository;
        this.jwtSettings = jwtSettingsOptions.Value;
    }

    private AuthResponse GenerateAuthResponse(string id, string username)
    {
        //Nei claims puoi aggiungere tutte le informazioni utili che ritieni possano servirti
        //User id del db, username ecc.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Name, username)
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

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var result = await authRepository.GetUserAsync(request.Username, request.Password);
        if (result == null)
            return null;

        return GenerateAuthResponse(result.ID, result.Username);
    }

    public async Task<AuthResponse> SigninAsync(LoginRequest request)
    {
        var id = Guid.NewGuid().ToString();
        await authRepository.InsertUserAsync(id, request.Username, request.Password);
        return GenerateAuthResponse(id, request.Username);
    }
}
