using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public interface IImageRepository
{
    Task AddLikeToImage(string currentUserID, string friendUserID);
    Task DeleteImage(string ImageID);
    Task<Image> GetImageInfo(string ImageID);
    Task<IEnumerable<Image>> GetImagesFromUsername(string username);
    Task SaveImage(Image image);
    Task RemoveLikeToImage(string currentUserID, string friendUserID);
}
