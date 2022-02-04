using System;
using System.Data;
using Dapper;
using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public class AuthRepository : Repository, IAuthRepository
{
    private readonly string connectionString;

    public AuthRepository(string connectionString) : base(connectionString)
    {
        this.connectionString = connectionString;
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

        return await conn.QueryFirstOrDefaultAsync<User>(sql, dynamicParameters);
    }

    public async Task InsertUserAsync(User user)
    {
        var sql = @"INSERT INTO `users` (`ID`, `Username`, `Password`, `Firstname`, `Lastname`) VALUES
                  (@ID, @USERNAME, @PASSWORD, @FIRSTNAME, @LASTNAME);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", user.ID, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@USERNAME", user.Username, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PASSWORD", user.Password, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@FIRSTNAME", user.Firstname, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@LASTNAME", user.Lastname, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }
}
