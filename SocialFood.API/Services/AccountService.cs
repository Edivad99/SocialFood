using System;
using System.Collections.Generic;
using SocialFood.API.Extensions;
using SocialFood.Data.Entity;
using SocialFood.Data.Repository;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository accountRespository;

    public AccountService(IAccountRepository accountRespository)
    {
        this.accountRespository = accountRespository;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersFromUsernameAsync(string username)
    {
        username = username.Replace("%", "");
        username = username.Replace("_", "");
        if (username.Length < 3)
            return new List<UserDTO>();
        var result = await accountRespository.GetUserFromUsernameAsync(username);
        return result.Select(u => u.ToUserDTO());
    }

    public async Task<IEnumerable<UserDTO>> GetUsersFriendsAsync(string username)
    {
        var result = await accountRespository.GetUsersFriendsAsync(username);
        if (result.Any())
            return result.Select(u => u.ToUserDTO());
        return new List<UserDTO>();
    }
}

