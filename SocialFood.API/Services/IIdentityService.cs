using System;
using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IIdentityService
{
    Task<Response<AuthResponse?>> LoginAsync(LoginRequest request);
    Task<Response<AuthResponse>> RegistrationAsync(RegistrationRequest request);
}