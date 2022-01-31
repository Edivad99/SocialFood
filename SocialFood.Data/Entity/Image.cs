using System.ComponentModel.DataAnnotations.Schema;

namespace SocialFood.Data.Entity;

public class Image
{
    public string Id { get; set; }
    public string IdUser { get; set; }
    public string Path { get; set; }
    public int Length { get; set; }
    public DateTime Ora { get; set; }
    public string Descrizione { get; set; }
    public string Luogo { get; set; }
}
