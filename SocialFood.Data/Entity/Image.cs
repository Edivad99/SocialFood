namespace SocialFood.Data.Entity;

public class Image
{
    public string Id { get; set; } = string.Empty;
    public string IdUser { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int Length { get; set; }
    public DateTime Ora { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public string Luogo { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
