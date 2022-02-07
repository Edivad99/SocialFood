using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IImageService
{
    Task<Response> AddLikeToImageAsync(Guid userID, Guid imageID);
    Task<Response<ImageDTO?>> DeleteAsync(Guid userID, Guid imageID);
    Task<(Stream Stream, string ContentType)?> GetImageAsync(Guid imageID);
    Task<Response<ImageDTO?>> GetImageInfoAsync(Guid userID, Guid imageID);
    Task<Response<IEnumerable<ImageDTO>>> GetImageInfoFromUsernameAsync(Guid userID, string username);
    Task<Response<IEnumerable<ImageDTO>>> GetLatestImagesFromFriendsAsync(Guid userID);
    Task<Response> RemoveLikeToImageAsync(Guid userID, Guid imageID);
    Task<Response> UploadAsync(Guid IdUser, string username, StreamFileContent file, string descrizione, DateTime ora, string luogo);
}
