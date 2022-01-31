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
    public async Task<IActionResult> GetImage(string imageID)
    {
        var image = await imageService.GetImageAsync(imageID);
        if (image == null)
            return NotFound();
        return File(image.Value.Stream, image.Value.ContentType);
    }

    [HttpGet("info/{imageID}")]
    public async Task<IActionResult> GetImageInfo(string imageID)
    {
        var image = await imageService.GetImageInfoAsync(imageID);
        if(image == null)
            return NotFound();
        return Ok(image);
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadImageRequest request)
    {
        var streamFileContent = new StreamFileContent(request.File.OpenReadStream(), request.File.ContentType,
            request.File.FileName, Convert.ToInt32(request.File.Length));

        await imageService.UploadAsync(User.GetId(), streamFileContent, request.Descrizione, request.Ora, request.Luogo);
        return NoContent();
    }

    [HttpDelete("{imageID}")]
    public async Task<IActionResult> Delete(string imageID)
    {
        var image = await imageService.DeleteAsync(User.GetId(), imageID);
        if (image == null)
            return NotFound();
        return Ok(image);
    }
}
