using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository
{
    public interface IAuthRepository
    {
        Task<User?> GetUserAsync(string username, string password);
        Task InsertUserAsync(string id, string username, string password);
    }
}