using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IAccountService
{
    Task<IEnumerable<UserDTO>> GetUsersFriendsAsync(string username);
    Task<IEnumerable<UserDTO>> GetUsersFromUsernameAsync(string username);
}
