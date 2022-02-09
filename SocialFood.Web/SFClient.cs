using System.Net.Http.Json;
using SocialFood.Shared.Models;

namespace SocialFood.Web;

public class SFClient
{
    private readonly HttpClient httpClient;

    public SFClient(HttpClient httpClient) => this.httpClient = httpClient;

    private async Task<List<T>> GetListAsync<T>(string url) where T : class
    {
        var responseMessage = await httpClient.GetAsync(url);
        if (responseMessage.IsSuccessStatusCode)
            return await responseMessage.Content.ReadFromJsonAsync<List<T>>() ?? new();
        else
            return new();
    }

    public Task<List<ImageDTO>> GetLatestImagesAsync() => GetListAsync<ImageDTO>("/api/Image/latest");

    public Task<List<UserDTO>> GetMyFriendsAsync() => GetListAsync<UserDTO>("/api/Account/me/friends");

    public Task<List<UserDTO>> GetUserByUsername(string username) => GetListAsync<UserDTO>($"/api/Account/finduser/{username}");

    public Task<List<ImageDTO>> GetImagesByUsername(string username) => GetListAsync<ImageDTO>($"/api/Image/images/{username}");
}
