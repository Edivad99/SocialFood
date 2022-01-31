using SocialFood.Data.Entity;
using SocialFood.Shared.Models;

namespace SocialFood.API.Extensions;

public static class ImageExtensions
{
    public static ImageDTO ToImageDTO(this Image image)
    {
        return new()
        {
            Id = image.Id,
            Descrizione = image.Descrizione,
            Name = Path.GetFileName(image.Path),
            Length = image.Length,
            Luogo = image.Luogo,
            Ora = image.Ora,
            ContentType = MimeMapping.MimeUtility.GetMimeMapping(image.Path)
        };
    }
}
