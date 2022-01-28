using SocialFood.Data.Entity;
using SocialFood.Data.Repository;

namespace SocialFood.Data.Repository;

public interface IAccountRepository
{
    Task<IEnumerable<User>> GetUserFromUsernameAsync(string username);
}
