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
        var sql = @"SELECT Id, Username
                    FROM users 
                    WHERE Username LIKE CONCAT(@USERNAME,'%');";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql, dynamicParameters);
    }
}

