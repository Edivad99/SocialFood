using SocialFood.Data.Entity;
using SocialFood.Data.Repository;

namespace SocialFood.Data.Repository;

public interface IAccountRepository
{
    Task AddFriendAsync(string currentUserID, string friendUserID);
    Task<IEnumerable<User>> GetUserFromUsernameAsync(string username);
    Task<IEnumerable<User>> GetUsersFollowersAsync(string userID);
    Task<IEnumerable<User>> GetUsersFriendsAsync(string username);
    Task RemoveFriendAsync(string currentUserID, string friendUserID);
}
