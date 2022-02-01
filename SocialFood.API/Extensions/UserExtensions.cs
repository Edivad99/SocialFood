using SocialFood.Data.Entity;
using SocialFood.Shared.Models;

namespace SocialFood.API.Extensions;

public static class UserExtensions
{
    public static UserDTO ToUserDTO(this User user)
    {
        return new()
        {
            Username = user.Username,
            Name = user.Firstname,
            Surname = user.Lastname
        };
    }
}
