using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IImageService
{
    Task<bool> AddLikeToImage(Guid userID, Guid imageID);
    Task<ImageDTO?> DeleteAsync(Guid userID, Guid imageID);
    Task<(Stream Stream, string ContentType)?> GetImageAsync(Guid imageID);
    Task<ImageDTO?> GetImageInfoAsync(Guid imageID);
    Task<IEnumerable<ImageDTO>> GetImageInfoFromUsernameAsync(string username);
    Task<bool> RemoveLikeToImage(Guid userID, Guid imageID);
    Task UploadAsync(Guid IdUser, StreamFileContent file, string descrizione, DateTime ora, string luogo);
}
