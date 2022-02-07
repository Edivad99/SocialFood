using System;
using System.Data;
using Dapper;
using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public class NotificationRepository : Repository, INotificationRepository
{
    public NotificationRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<Notification>> GetNotificationSubscriptionAsync(string IDUser)
    {
        var sql = @"SELECT *
                    FROM notifications 
                    WHERE IDUser = @IDUSER;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDUSER", IDUser, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<Notification>(sql, dynamicParameters);
    }

    public async Task AddNotificationSubscriptionAsync(Notification notification)
    {
        var sql = @"INSERT INTO `notifications` (`IDUser`, `Url`, `P256dh`, `Auth`) VALUES
                  (@IDUSER, @URL, @P256DH, @AUTH);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDUSER", notification.IDUser, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@URL", notification.Url, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@P256DH", notification.P256dh, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@AUTH", notification.Auth, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task RemoveNotificationSubscriptionByIdAsync(string IDUser)
    {
        var sql = @"DELETE FROM `notifications` WHERE `IDUser` = @IDUSER;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDUSER", IDUser, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task RemoveNotificationSubscriptionByUrlAsync(string url)
    {
        var sql = @"DELETE FROM `notifications` WHERE `Url` = @URL;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@URL", url, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }
}

