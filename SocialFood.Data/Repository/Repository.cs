using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace SocialFood.Data.Repository;

public abstract class Repository
{
    private readonly string ConnectionString;

    public Repository(string connectionString) => ConnectionString = connectionString;

    protected IDbConnection GetDbConnection() => new MySqlConnection(ConnectionString);
}
