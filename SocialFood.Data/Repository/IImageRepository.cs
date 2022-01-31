using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public interface IImageRepository
{
    Task DeleteImage(string ImageID);
    Task<Image> GetImageInfo(string ImageID);
    Task<IEnumerable<Image>> GetImagesFromUserID(string UserID);
    Task SaveImage(Image image);
}
