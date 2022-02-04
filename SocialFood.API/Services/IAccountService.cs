using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IAccountService
{
    Task<Response> AddFriendAsync(Guid currentUserID, string friendUsername);
    Task<Response<IEnumerable<UserDTO>>> GetUsersFriendsAsync(string username);
    Task<Response<IEnumerable<UserDTO>>> GetUsersFromUsernameAsync(string username);
    Task<Response> RemoveFriendAsync(Guid currentUserID, string friendUsername);
}
