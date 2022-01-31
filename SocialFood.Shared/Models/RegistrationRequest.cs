using System.ComponentModel.DataAnnotations;

namespace SocialFood.Shared.Models;

public class RegistrationRequest : LoginRequest
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}
