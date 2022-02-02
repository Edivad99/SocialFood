namespace SocialFood.Shared.Models;

public class ImageDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Length { get; set; }
    public DateTime Ora { get; set; }
    public string Descrizione { get; set; }
    public string Luogo { get; set; }
    public string ContentType { get; set; }
    public bool YourLike { get; set; }
    public int Likes { get; set; }
    public string Username { get; set; }
}
