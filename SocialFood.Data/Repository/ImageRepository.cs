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
        var sql = @"SELECT * FROM `images` WHERE `ID` = @ID;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", ImageID, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<Image>(sql, dynamicParameters);
    }

    public async Task<IEnumerable<Image>> GetImagesFromUserID(string UserID)
    {
        var sql = @"SELECT * FROM `images` WHERE `IDUser` = @ID ORDER BY `Ora` DESC;";

        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add("@ID", UserID, DbType.String, ParameterDirection.Input);

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
}

