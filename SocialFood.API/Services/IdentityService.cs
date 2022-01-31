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

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await authRepository.GetUserAsync(request.Username, request.Password);
        if (user == null)
            return null;

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RegistrationAsync(RegistrationRequest request)
    {
        var id = Guid.NewGuid().ToString();
        var user = new User(id, request.Username, request.Password, request.FirstName, request.LastName);
        await authRepository.InsertUserAsync(user);
        return GenerateAuthResponse(user);
    }
}
