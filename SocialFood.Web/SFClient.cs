using System.Net.Http.Json;
using System.Net;
using SocialFood.Shared.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace SocialFood.Web;

public class SFClient
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider stateProvider;

    public SFClient(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider stateProvider)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
        this.stateProvider = stateProvider;
    }

    private async Task<List<T>> GetListAsync<T>(string url) where T : class
    {
        var responseMessage = await httpClient.GetAsync(url);
        if (responseMessage.IsSuccessStatusCode)
            return await responseMessage.Content.ReadFromJsonAsync<List<T>>() ?? new();
        else if(responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            await localStorage.RemoveItemAsync("token");
            await stateProvider.GetAuthenticationStateAsync();
        }
        return new();
    }

    public Task<List<ImageDTO>> GetLatestImagesAsync() => GetListAsync<ImageDTO>("/api/Image/latest");

    public Task<List<UserDTO>> GetMyFriendsAsync() => GetListAsync<UserDTO>("/api/Account/me/friends");

    public Task<List<UserDTO>> GetUserByUsername(string username) => GetListAsync<UserDTO>($"/api/Account/finduser/{username}");

    public Task<List<ImageDTO>> GetImagesByUsername(string username) => GetListAsync<ImageDTO>($"/api/Image/images/{username}");
}
