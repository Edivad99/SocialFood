using System;
using System.Collections.Generic;
using SocialFood.Data.Entity;
using SocialFood.Data.Repository;

namespace SocialFood.API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository accountRespository;

    public AccountService(IAccountRepository accountRespository)
    {
        this.accountRespository = accountRespository;
    }

    public async Task<IEnumerable<User>> GetUserFromUsernameAsync(string username)
    {
        username = username.Replace("%", "");
        username = username.Replace("_", "");
        if (username.Length < 3)
            return new List<User>();
        var result = await accountRespository.GetUserFromUsernameAsync(username);
        return result;
    }
}

