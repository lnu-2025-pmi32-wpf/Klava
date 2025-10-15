using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class UserManager
{
    private readonly DbManager _dbManager;

    public UserManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateUser(string firstName, string lastName, string password)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Users (firstname, lastname, password) VALUES (@firstName, @lastName, @password)", 
            connection);

        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@password", password);

        command.ExecuteNonQuery();
    }

    public User? GetUserById(int userId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetUserById(@userId)", connection);
        command.Parameters.AddWithValue("userId", userId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2)
            };
        }

        return null;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT id, firstname, lastname FROM Users ORDER BY id", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2)
            });
        }

        return users;
    }

    public void UpdateUser(int userId, string firstName, string lastName, string password)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE Users SET firstname = @firstName, lastname = @lastName, password = @password WHERE id = @userId", 
            connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@password", password);

        command.ExecuteNonQuery();
    }

    public void DeleteUser(int userId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM Users WHERE id = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.ExecuteNonQuery();
    }
}
