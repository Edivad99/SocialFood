using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public interface IImageRepository
{
    Task AddLikeToImage(string currentUserID, string friendUserID);
    Task DeleteImage(string ImageID);
    Task<Image> GetImageInfo(string ImageID);
    Task<IEnumerable<Image>> GetImagesFromUsername(string username);
    Task<IEnumerable<User>> GetImageLikes(string imageID);
    Task SaveImage(Image image);
    Task RemoveLikeToImage(string currentUserID, string friendUserID);
}
