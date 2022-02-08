using System;
using System.Collections.Generic;
using SocialFood.API.Extensions;
using SocialFood.API.Models;
using SocialFood.Data.Entity;
using SocialFood.Data.Repository;
using SocialFood.Shared.Models;

namespace SocialFood.API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository accountRespository;
    private readonly INotificationService notificationService;
    private readonly ILogger<AccountService> logger;

    public AccountService(IAccountRepository accountRespository, INotificationService notificationService , ILogger<AccountService> logger)
    {
        this.accountRespository = accountRespository;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task<Response<IEnumerable<UserDTO>>> GetUsersFromUsernameAsync(string username)
    {
        try
        {
            logger.LogInformation($"New GetUsersFromUsername request: {username}");
            username = username.Replace("%", "");
            username = username.Replace("_", "");
            if (username.Length < 3)
                return new() { StatusCode = StatusCodes.Status404NotFound, Result = new List<UserDTO>() };
            var users = await accountRespository.GetUserFromUsernameAsync(username);
            var usersDTO = users.Select(u => u.ToUserDTO());

            var response = new Response<IEnumerable<UserDTO>>()
            {
                StatusCode = usersDTO.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound,
                Result = usersDTO
            };
            logger.LogInformation($"GetUsersFromUsername response with status: {response.StatusCode}");
            return response;
        }
        catch(Exception e)
        {
            logger.LogError(e, $"New error in GetUsersFromUsernameAsync with username: { username }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = new List<UserDTO>() };
        }
        
    }

    public async Task<Response<IEnumerable<UserDTO>>> GetUsersFriendsAsync(string username)
    {
        try
        {
            logger.LogInformation($"New GetUsersFriendsAsync request: {username}");
            var users = await accountRespository.GetUsersFriendsAsync(username);
            var usersDTO = users.Select(u => u.ToUserDTO());

            var response = new Response<IEnumerable<UserDTO>>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = usersDTO
            };
            logger.LogInformation($"GetUsersFriendsAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in GetUsersFriendsAsync with username: { username }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = new List<UserDTO>() };
        } 
    }

    public async Task<Response<IEnumerable<UserDTO>>> GetUsersFollowersAsync(Guid userID)
    {
        try
        {
            logger.LogInformation($"New GetUsersFollowersAsync request: {userID}");
            var users = await accountRespository.GetUsersFollowersAsync(userID.ToString());
            var usersDTO = users.Select(u => u.ToUserDTO());

            var response = new Response<IEnumerable<UserDTO>>()
            {
                StatusCode = StatusCodes.Status200OK,
                Result = usersDTO
            };
            logger.LogInformation($"GetUsersFollowersAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in GetUsersFollowersAsync with username: { userID }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError, Result = new List<UserDTO>() };
        }
    }

    private async Task<Response> ManageFriendships(string friendUsername, Action<string> action)
    {
        var users = await accountRespository.GetUserFromUsernameAsync(friendUsername);
        var friend = users.FirstOrDefault();

        Response response;
        if (users.Count() == 1 && friend != null)
        {
            action.Invoke(friend.ID);
            response = new() { StatusCode = StatusCodes.Status200OK };
        }
        else
            response = new() { StatusCode = StatusCodes.Status400BadRequest };
        return response;
    }

    public async Task<Response> AddFriendAsync(Guid currentUserID, string currentUsername, string friendUsername)
    {
        try
        {
            logger.LogInformation($"New AddFriendAsync request: {friendUsername}");

            var response = await ManageFriendships(friendUsername, async friendUserID =>
            {
                await accountRespository.AddFriendAsync(currentUserID.ToString(), friendUserID);

                await notificationService.NotificationNewFriendship(Guid.Parse(friendUserID), currentUsername);
            });
            logger.LogInformation($"AddFriendAsync response with status: {response.StatusCode}");
            return response;
        }
        catch(Exception e)
        {
            logger.LogError(e, $"New error in AddFriendAsync with friendUsername: { friendUsername }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public async Task<Response> RemoveFriendAsync(Guid currentUserID, string friendUsername)
    {
        try
        {
            logger.LogInformation($"New RemoveFriendAsync request: {friendUsername}");

            var response = await ManageFriendships(friendUsername, async friendUserID =>
            {
                await accountRespository.RemoveFriendAsync(currentUserID.ToString(), friendUserID);
            });
            logger.LogInformation($"RemoveFriendAsync response with status: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"New error in RemoveFriendAsync with friendUsername: { friendUsername }");
            return new() { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}

