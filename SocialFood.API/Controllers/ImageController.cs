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
    private readonly ILogger<ImageController> logger;

    public ImageController(IImageService imageService, ILogger<ImageController> logger)
    {
        this.imageService = imageService;
        this.logger = logger;
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
        if (image == null)
            return NotFound();
        return Ok(image);
    }

    [HttpPut("{imageID}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLikeToImage(Guid imageID)
    {
        var response = await imageService.AddLikeToImage(User.GetId(), imageID);
        return response ? Ok() : BadRequest();
    }

    [HttpDelete("{imageID}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveLikeToImage(Guid imageID)
    {
        var response = await imageService.RemoveLikeToImage(User.GetId(), imageID);
        return response ? Ok() : BadRequest();
    }

    [HttpGet("{imageID}/info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageInfo(Guid imageID)
    {
        var image = await imageService.GetImageInfoAsync(User.GetId(), imageID);
        if(image == null)
            return NotFound();
        return Ok(image);
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Upload([FromForm] UploadImageRequest request)
    {
        try
        {
            var streamFileContent = new StreamFileContent(request.File.OpenReadStream(), request.File.ContentType,
            request.File.FileName, Convert.ToInt32(request.File.Length));

            await imageService.UploadAsync(User.GetId(), streamFileContent, request.Descrizione, request.Ora, request.Luogo);
            return NoContent();
        } catch(Exception e)
        {
            return StatusCode(500, e.Message + "\n" + e.StackTrace);
        }
        
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
        var images = await imageService.GetImageInfoFromUsernameAsync(User.GetId(), username);
        if (images == null)
            return NotFound();
        return Ok(images);
    }

    [HttpGet("latest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLatestImagesFromFriends()
    {
        var images = await imageService.GetLatestImagesFromFriendsAsync(User.GetId());
        return Ok(images);
    }
}
