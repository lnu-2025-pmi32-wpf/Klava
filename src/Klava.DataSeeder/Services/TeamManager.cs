using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class TeamManager
{
    private readonly DbManager _dbManager;

    public TeamManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateTeam(string teamName, string accessCode, int ownerId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Teams (name, code, owner_id) VALUES (@name, @code, @ownerId)", 
            connection);

        command.Parameters.AddWithValue("@name", teamName);
        command.Parameters.AddWithValue("@code", accessCode);
        command.Parameters.AddWithValue("@ownerId", ownerId);

        command.ExecuteNonQuery();
    }

    public Team? GetTeamById(int teamId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetTeamById(@teamId)", connection);
        command.Parameters.AddWithValue("teamId", teamId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Team
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Code = reader.GetString(2),
                OwnerId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
            };
        }

        return null;
    }

    public List<Team> GetAllTeams()
    {
        var teams = new List<Team>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT id, name, code, owner_id FROM Teams ORDER BY id", connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            teams.Add(new Team
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Code = reader.GetString(2),
                OwnerId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
            });
        }

        return teams;
    }

    public List<Team> GetTeamsForUser(int userId)
    {
        var teams = new List<Team>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetTeamsForUser(@userId)", connection);
        command.Parameters.AddWithValue("userId", userId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            teams.Add(new Team
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Code = reader.GetString(2)
            });
        }

        return teams;
    }

    public void UpdateTeam(int teamId, string newName, string newCode)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE Teams SET name = @name, code = @code WHERE id = @teamId", 
            connection);

        command.Parameters.AddWithValue("@teamId", teamId);
        command.Parameters.AddWithValue("@name", newName);
        command.Parameters.AddWithValue("@code", newCode);

        command.ExecuteNonQuery();
    }

    public void DeleteTeam(int teamId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM Teams WHERE id = @teamId", connection);
        command.Parameters.AddWithValue("@teamId", teamId);
        command.ExecuteNonQuery();
    }
}
