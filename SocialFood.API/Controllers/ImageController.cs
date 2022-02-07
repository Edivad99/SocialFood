using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialFood.API.Models;
using SocialFood.API.Services;
using SocialFood.Shared.Extensions;

namespace SocialFood.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageService imageService;

    public ImageController(IImageService imageService)
    {
        this.imageService = imageService;
    }

    [HttpGet("{imageID}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(Guid imageID)
    {
        var image = await imageService.GetImageAsync(imageID);
        if (image == null)
            return NotFound();
        return File(image.Value.Stream, image.Value.ContentType);
    }

    [HttpDelete("{imageID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid imageID)
    {
        var image = await imageService.DeleteAsync(User.GetId(), imageID);
        return StatusCode(image.StatusCode, image);
    }

    [HttpPut("{imageID}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLikeToImage(Guid imageID)
    {
        var response = await imageService.AddLikeToImageAsync(User.GetId(), imageID);
        return StatusCode(response.StatusCode);
    }

    [HttpDelete("{imageID}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveLikeToImage(Guid imageID)
    {
        var response = await imageService.RemoveLikeToImageAsync(User.GetId(), imageID);
        return StatusCode(response.StatusCode);
    }

    [HttpGet("{imageID}/info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageInfo(Guid imageID)
    {
        var response = await imageService.GetImageInfoAsync(User.GetId(), imageID);
        return StatusCode(response.StatusCode, response.Result);
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Upload([FromForm] UploadImageRequest request)
    {
        var streamFileContent = new StreamFileContent(request.File.OpenReadStream(), request.File.ContentType,
        request.File.FileName, Convert.ToInt32(request.File.Length));

        var result = await imageService.UploadAsync(User.GetId(), User.GetUsername()!, streamFileContent, request.Descrizione, request.Ora, request.Luogo);
        return StatusCode(result.StatusCode);        
    }

    [HttpGet("images/me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyImages() => await GetImagesFromUsername(User.GetUsername()!);

    [HttpGet("images/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImagesFromUsername(string username)
    {
        var result = await imageService.GetImageInfoFromUsernameAsync(User.GetId(), username);
        return StatusCode(result.StatusCode, result.Result);
    }

    [HttpGet("latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestImagesFromFriends()
    {
        var images = await imageService.GetLatestImagesFromFriendsAsync(User.GetId());
        return StatusCode(images.StatusCode, images.Result);
    }
}
