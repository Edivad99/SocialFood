using System;
using System.Data;
using Dapper;
using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public class AccountRepository : Repository, IAccountRepository
{
    public AccountRepository(string connectionString) : base(connectionString)
    {
    }


    public async Task<IEnumerable<User>> GetUserFromUsernameAsync(string username)
    {
        var sql = @"SELECT *
                    FROM users 
                    WHERE Username LIKE CONCAT(@USERNAME,'%');";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql, dynamicParameters);
    }

    public async Task<IEnumerable<User>> GetUsersFriendsAsync(string username)
    {
        var sql = @"SELECT *
                    FROM `users`
                    WHERE `ID` IN (
                        SELECT `IDUserB` 
                        FROM `friendships`
                        INNER JOIN `users`
                        ON users.`ID` = friendships.`IDUserA`
                        WHERE `Username` = @USERNAME)";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql, dynamicParameters);
    }


    private async Task ManageFriendships(string sql, string currentUserID, string friendUserID)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDUSERA", currentUserID, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@IDUSERB", friendUserID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task AddFriendAsync(string currentUserID, string friendUserID)
    {
        var sql = @"INSERT INTO `friendships` (`IDUserA`, `IDUserB`) VALUES
                  (@IDUSERA, @IDUSERB);";
        await ManageFriendships(sql, currentUserID, friendUserID);
    }

    public async Task RemoveFriendAsync(string currentUserID, string friendUserID)
    {
        var sql = @"DELETE FROM `friendships` WHERE `IDUserA` = @IDUSERA AND `IDUserB` = @IDUSERB;";
        await ManageFriendships(sql, currentUserID, friendUserID);
    }
}

