using SocialFood.Data.Entity;
using SocialFood.Shared.Models;

namespace SocialFood.API.Extensions;

public static class ImageExtensions
{
    public static string GetMimeMapping(this Image image) => MimeMapping.MimeUtility.GetMimeMapping(image.Path);
}
