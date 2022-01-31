using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SocialFood.API.Filters;

namespace SocialFood.API.Models;

public class UploadImageRequest
{
    [BindRequired]
    [AllowedExtensions("jpeg", "jpg", "png")]
    public IFormFile File { get; set; }
    [Required]
    public string Descrizione { get; set; }
    [Required]
    public DateTime Ora { get; set; }
    [Required]
    public string Luogo { get; set; }
}
