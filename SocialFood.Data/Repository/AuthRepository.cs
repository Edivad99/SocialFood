using System;
using System.Data;
using Dapper;
using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public class AuthRepository : Repository, IAuthRepository
{
    public AuthRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<User?> GetUserAsync(string username, string password)
    {
        var sql = @"SELECT * 
                    FROM users 
                    WHERE Username = @USERNAME AND Password = @PASSWORD;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", password, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryFirstAsync<User>(sql, dynamicParameters);
    }

    public async Task InsertUserAsync(string id, string username, string password)
    {
        var sql = @"INSERT INTO `users` (`ID`, `Username`, `Password`) VALUES
                  (@ID, @USERNAME, @PASSWORD);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", id, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", password, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }
}
