using System;
using System.Collections.Generic;
using SocialFood.API.Extensions;
using SocialFood.API.Models;
using SocialFood.Data.Entity;
using SocialFood.Data.Repository;
using SocialFood.Shared.Models;
using SocialFood.StorageProviders;

namespace SocialFood.API.Services;

public class ImageService : IImageService
{
    private readonly IImageRepository imageRepository;
    private readonly IStorageProvider storageProvider;

    public ImageService(IImageRepository imageRepository, IStorageProvider storageProvider)
    {
        this.imageRepository = imageRepository;
        this.storageProvider = storageProvider;
    }

    private static string BuildPath(Guid IdUser, string fileName) =>
        Path.Combine(IdUser.ToString(), fileName);

    public async Task UploadAsync(Guid IdUser, StreamFileContent file, string descrizione, DateTime ora, string luogo)
    {
        var path = BuildPath(IdUser, file.FileName);
        await storageProvider.SaveAsync(path, file.Content);

        var image = new Image()
        {
            Id = Guid.NewGuid().ToString(),
            IdUser = IdUser.ToString(),
            Path = path,
            Length = file.Length,
            Descrizione = descrizione,
            Ora = ora,
            Luogo = luogo
        };
        await imageRepository.SaveImage(image);
    }

    public async Task<ImageDTO?> DeleteAsync(Guid userID, Guid imageID)
    { 
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return null;
        if (image.IdUser != userID.ToString())
            return null;
        await imageRepository.DeleteImage(imageID.ToString());
        await storageProvider.DeleteAsync(image.Path);
        return image.ToImageDTO();
    }

    private async Task<Image> GetImageFullInfoAsync(Guid imageID) => await imageRepository.GetImageInfo(imageID.ToString());

    public async Task<ImageDTO?> GetImageInfoAsync(Guid imageID)
    {
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return null;
        return image.ToImageDTO();
    }

    public async Task<(Stream Stream, string ContentType)?> GetImageAsync(Guid imageID)
    {
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return null;
        var stream = await storageProvider.ReadAsync(image.Path);
        return (stream, image.ToImageDTO().ContentType);
    }

    public async Task<IEnumerable<ImageDTO>> GetImageInfoFromUsernameAsync(string username)
    {
        var images = await imageRepository.GetImagesFromUsername(username);
        return images.Select(x => x.ToImageDTO());
    }

    public async Task<bool> AddLikeToImage(Guid userID, Guid imageID)
    {
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return false;
        await imageRepository.AddLikeToImage(userID.ToString(), imageID.ToString());
        return true;
    }

    public async Task<bool> RemoveLikeToImage(Guid userID, Guid imageID)
    {
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return false;
        await imageRepository.RemoveLikeToImage(userID.ToString(), imageID.ToString());
        return true;
    }
}

