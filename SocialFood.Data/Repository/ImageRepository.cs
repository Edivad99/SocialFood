using System;
using System.Data;
using Dapper;
using SocialFood.Data.Entity;

namespace SocialFood.Data.Repository;

public class ImageRepository : Repository, IImageRepository
{
    public ImageRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task SaveImage(Image image)
    {
        var sql = @"INSERT INTO `images` (`ID`, `IDUser`, `Path`, `Length`, `Ora`, `Descrizione`, `Luogo`) VALUES
                    (@ID, @IDUSER, @PATH, @LENGHT, @ORA, @DESCRIZIONE, @LUOGO);";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", image.Id, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@IDUSER", image.IdUser, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@PATH", image.Path, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@LENGHT", image.Length, DbType.Int32, ParameterDirection.Input);
        dynamicParameters.Add("@ORA", image.Ora, DbType.DateTime, ParameterDirection.Input);
        dynamicParameters.Add("@DESCRIZIONE", image.Descrizione, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@LUOGO", image.Luogo, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task<Image> GetImageInfo(string ImageID)
    {
        var sql = @"SELECT `images`.*, `users`.Username
                    FROM `images`
                    INNER JOIN `users`
                    ON users.`ID` = `images`.`IDUser`
                    WHERE `images`.`ID` = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", ImageID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<Image>(sql, dynamicParameters);
    }

    public async Task<IEnumerable<Image>> GetImagesFromUsername(string username)
    {
        var sql = @"SELECT `images`.*, `users`.Username
                    FROM `images`
                    INNER JOIN `users`
                    ON users.`ID` = `images`.`IDUser`
                    WHERE `Username` = @USERNAME
                    ORDER BY `Ora` DESC;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERNAME", username, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<Image>(sql, dynamicParameters);
    }

    public async Task DeleteImage(string ImageID)
    {
        var sql = @"DELETE FROM `images` WHERE `ID` = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", ImageID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    private async Task ManageLikes(string sql, string currentUserID, string imageID)
    {
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDUSER", currentUserID, DbType.String, ParameterDirection.Input);
        dynamicParameters.Add("@IDIMAGE", imageID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynamicParameters);
    }

    public async Task AddLikeToImage(string currentUserID, string friendUserID)
    {
        var sql = @"INSERT INTO `likes` (`IDUser`, `IDImage`) VALUES
                  (@IDUSER, @IDIMAGE);";
        await ManageLikes(sql, currentUserID, friendUserID);
    }

    public async Task RemoveLikeToImage(string currentUserID, string friendUserID)
    {
        var sql = @"DELETE FROM `likes` WHERE `IDUser` = @IDUSER AND `IDImage` = @IDIMAGE;";
        await ManageLikes(sql, currentUserID, friendUserID);
    }

    public async Task<IEnumerable<User>> GetImageLikes(string imageID)
    {
        var sql = @"SELECT `users`.* 
                    FROM `users`
                    INNER JOIN `likes`
                    ON `likes`.`IDUser` = `users`.`ID`
                    WHERE likes.`IDImage` = @IDIMAGE;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@IDIMAGE", imageID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql, dynamicParameters);
    }

    public async Task<IEnumerable<Image>> GetLatestImagesFromFriends(string currentUserID)
    {
        var sql = @"SELECT `images`.*, `users`.Username
                    FROM `images`
                    INNER JOIN `users` ON `users`.`ID` = `images`.`IDUser`
                    WHERE images.`IDUser` IN (SELECT IDUserB
                        FROM `friendships`
                        WHERE `IDUserA` = @USERID)
                    ORDER BY `Ora` DESC;";
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@USERID", currentUserID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryAsync<Image>(sql, dynamicParameters);
    }
}

