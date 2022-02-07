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
    private readonly INotificationService notificationService;
    private readonly ILogger<ImageService> logger;

    public ImageService(IImageRepository imageRepository, IStorageProvider storageProvider, INotificationService notificationService, ILogger<ImageService> logger)
    {
        this.imageRepository = imageRepository;
        this.storageProvider = storageProvider;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    private static string BuildPath(Guid IdUser, string fileName) =>
        Path.Combine(IdUser.ToString(), fileName);

    public async Task<Response> UploadAsync(Guid IdUser, string username, StreamFileContent file, string descrizione, DateTime ora, string luogo)
    {
        try
        {
            logger.LogInformation($"New UploadAsync request with IdUser: {IdUser}");
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
            await notificationService.NotificationNewPhoto(IdUser, username);
            var response = new Response { StatusCode = StatusCodes.Status204NoContent };

            logger.LogInformation($"UploadAsync response with status: { response.StatusCode }");
            return response;
        }
        catch(Exception e)
        {
            logger.LogError(e, $"New error in UploadAsync with IdUser: { IdUser }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public async Task<Response<ImageDTO?>> DeleteAsync(Guid userID, Guid imageID)
    {
        try
        {
            logger.LogInformation($"New DeleteAsync request with IdUser: { userID } and imageID: { imageID }");
            var image = await GetImageFullInfoAsync(imageID);
            Response<ImageDTO?> response;

            if (image == null || (image.IdUser != userID.ToString()))
                response = new(){ StatusCode = StatusCodes.Status404NotFound, Result = null };
            else
            {
                await imageRepository.DeleteImage(imageID.ToString());
                await storageProvider.DeleteAsync(image.Path);
                var imageDTO = await ConvertToImageDTO(userID, image, true);
                response = new Response<ImageDTO?>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Result = imageDTO
                };
            }

            logger.LogInformation($"DeleteAsync response with status: { response.StatusCode }");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in DeleteAsync with IdUser: { userID } and imageID: { imageID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    private async Task<Image> GetImageFullInfoAsync(Guid imageID) => await imageRepository.GetImageInfo(imageID.ToString());

    public async Task<Response<ImageDTO?>> GetImageInfoAsync(Guid userID, Guid imageID)
    {
        try
        {
            logger.LogInformation($"New GetImageInfoAsync request with IdUser: { userID } and imageID: { imageID }");
            var image = await GetImageFullInfoAsync(imageID);
            Response<ImageDTO?> response;
            if (image == null)
                response = new() { StatusCode = StatusCodes.Status404NotFound, Result = null };
            else
            {
                response = new() { StatusCode = StatusCodes.Status200OK, Result = await ConvertToImageDTO(userID, image) };
            }
            
            logger.LogInformation($"GetImageInfoAsync response with status: { response.StatusCode }");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in GetImageInfoAsync with IdUser: { userID } and imageID: { imageID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public async Task<(Stream Stream, string ContentType)?> GetImageAsync(Guid imageID)
    {

        logger.LogInformation($"New GetImageAsync request: {imageID}");
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return null;

        var stream = await storageProvider.ReadAsync(image.Path);
        if (stream == null)
            return null;
        logger.LogInformation($"File retrived, GetImageAsync request: {imageID}");
        return (stream, image.GetMimeMapping());
    }

    public async Task<Response<IEnumerable<ImageDTO>>> GetImageInfoFromUsernameAsync(Guid userID, string username)
    {
        try
        {
            logger.LogInformation($"New GetImageInfoFromUsernameAsync request with username: { username }");
            var images = await imageRepository.GetImagesFromUsername(username);
            var imagesDTO =  images.Select(x => ConvertToImageDTO(userID, x)).Select(t => t.Result);

            Response<IEnumerable<ImageDTO>> response;
            if (imagesDTO.Any())
                response = new() { StatusCode = StatusCodes.Status200OK, Result = imagesDTO };
            else
                response = new() { StatusCode = StatusCodes.Status400BadRequest, Result = imagesDTO };

            logger.LogInformation($"GetImageInfoFromUsernameAsync response with status: { response.StatusCode }");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in GetImageInfoFromUsernameAsync with username: { username }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    private async Task<Response> ManageLikes(Guid imageID, Action action)
    {
        var image = await GetImageFullInfoAsync(imageID);
        if (image == null)
            return new() { StatusCode = StatusCodes.Status400BadRequest };
        action.Invoke();
        return new() { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<Response> AddLikeToImageAsync(Guid userID, Guid imageID)
    {
        try
        {
            logger.LogInformation($"New AddLikeToImageAsync request: {imageID}");

            var response = await ManageLikes(imageID, async () =>
            {
                await imageRepository.AddLikeToImage(userID.ToString(), imageID.ToString());
            });
            logger.LogInformation($"AddLikeToImageAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in AddLikeToImageAsync with imageID: { imageID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public async Task<Response> RemoveLikeToImageAsync(Guid userID, Guid imageID)
    {
        try
        {
            logger.LogInformation($"New RemoveLikeToImageAsync request: {imageID}");

            var response = await ManageLikes(imageID, async () =>
            {
                await imageRepository.RemoveLikeToImage(userID.ToString(), imageID.ToString());
            });
            logger.LogInformation($"RemoveLikeToImageAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in RemoveLikeToImageAsync with imageID: { imageID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public async Task<Response<IEnumerable<ImageDTO>>> GetLatestImagesFromFriendsAsync(Guid userID)
    {
        try
        {
            logger.LogInformation($"New GetLatestImagesFromFriendsAsync request with userID: { userID }");
            var images = await imageRepository.GetLatestImagesFromFriends(userID.ToString());
            var imagesDTO = images.Select(x => ConvertToImageDTO(userID, x)).Select(t => t.Result);

            var response = new Response<IEnumerable<ImageDTO>>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = imagesDTO
            };

            logger.LogInformation($"GetLatestImagesFromFriendsAsync response with status: { response.StatusCode }");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in GetLatestImagesFromFriendsAsync with userID: { userID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    private async Task<ImageDTO> ConvertToImageDTO(Guid userID, Image image, bool skipLikes = false)
    {
        var result = new ImageDTO()
        {
            Id = image.Id,
            Descrizione = image.Descrizione,
            Name = Path.GetFileName(image.Path),
            Length = image.Length,
            Luogo = image.Luogo,
            Ora = image.Ora,
            ContentType = image.GetMimeMapping(),
            Username = image.Username
        };

        if(!skipLikes)
        {
            var users = await imageRepository.GetImageLikes(image.Id);
            result.Likes = users.Count();
            result.YourLike = users.Where(u => u.ID == userID.ToString()).Any();
        }

        return result;
    }
}

