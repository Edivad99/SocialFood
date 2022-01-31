using SocialFood.API.Models;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public interface IImageService
{
    Task<ImageDTO?> DeleteAsync(Guid userID, string imageID);
    Task<(Stream Stream, string ContentType)?> GetImageAsync(string imageID);
    Task<ImageDTO?> GetImageInfoAsync(string imageID);
    Task UploadAsync(Guid IdUser, StreamFileContent file, string descrizione, DateTime ora, string luogo);
}
