using System;
namespace SocialFood.Data.Entity;

public record User
{
    public string ID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
