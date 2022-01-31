using System;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IIdentityService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegistrationAsync(RegistrationRequest request);
}