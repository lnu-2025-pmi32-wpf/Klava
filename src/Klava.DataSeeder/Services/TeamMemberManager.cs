using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class TeamMemberManager
{
    private readonly DbManager _dbManager;

    public TeamMemberManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void AddUserToTeam(int userId, int teamId, string role = "student")
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO TeamMembers (user_id, team_id, role) VALUES (@userId, @teamId, @role::team_member_role)", 
            connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@teamId", teamId);
        command.Parameters.AddWithValue("@role", role);

        command.ExecuteNonQuery();
    }

    public List<TeamMember> GetTeamMembers(int teamId)
    {
        var members = new List<TeamMember>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetTeamMembers(@teamId)", connection);
        command.Parameters.AddWithValue("teamId", teamId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            members.Add(new TeamMember
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Role = reader.GetString(3).ToLower() == "student" ? TeamMemberRole.Student : TeamMemberRole.Headman
            });
        }

        return members;
    }

    public void UpdateTeamMemberRole(int userId, int teamId, string newRole)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE TeamMembers SET role = @role::team_member_role WHERE user_id = @userId AND team_id = @teamId", 
            connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@teamId", teamId);
        command.Parameters.AddWithValue("@role", newRole);

        command.ExecuteNonQuery();
    }

    public void RemoveUserFromTeam(int userId, int teamId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "DELETE FROM TeamMembers WHERE user_id = @userId AND team_id = @teamId", 
            connection);

        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@teamId", teamId);

        command.ExecuteNonQuery();
    }
}
