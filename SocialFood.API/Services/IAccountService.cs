using SocialFood.Data.Entity;

namespace SocialFood.API.Services;

public interface IAccountService
{
    Task<IEnumerable<User>> GetUserFromUsernameAsync(string username);
}
