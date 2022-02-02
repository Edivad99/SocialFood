using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IAccountService
{
    Task<bool> AddFriendAsync(Guid currentUserID, string friendUsername);
    Task<IEnumerable<UserDTO>> GetUsersFriendsAsync(string username);
    Task<IEnumerable<UserDTO>> GetUsersFromUsernameAsync(string username);
    Task<bool> RemoveFriendAsync(Guid currentUserID, string friendUsername);
}
