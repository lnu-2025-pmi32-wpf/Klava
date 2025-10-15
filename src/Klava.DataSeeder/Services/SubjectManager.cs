using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class SubjectManager
{
    private readonly DbManager _dbManager;

    public SubjectManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateSubject(int teamId, string title, string info, string status)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Subjects (team_id, title, subject_info, status) VALUES (@teamId, @title, @info, @status::subject_status)", 
            connection);

        command.Parameters.AddWithValue("@teamId", teamId);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@info", info);
        command.Parameters.AddWithValue("@status", status);

        command.ExecuteNonQuery();
    }

    public Subject? GetSubjectById(int subjectId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetSubjectById(@subjectId)", connection);
        command.Parameters.AddWithValue("subjectId", subjectId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Subject
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                SubjectInfo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Status = reader.GetString(3).ToLower() == "exam" ? SubjectStatus.Exam : SubjectStatus.Test
            };
        }

        return null;
    }

    public List<Subject> GetSubjectsForTeam(int teamId)
    {
        var subjects = new List<Subject>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetSubjectsForTeam(@teamId)", connection);
        command.Parameters.AddWithValue("teamId", teamId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            subjects.Add(new Subject
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Status = reader.GetString(2).ToLower() == "exam" ? SubjectStatus.Exam : SubjectStatus.Test
            });
        }

        return subjects;
    }

    public void UpdateSubject(int subjectId, string newTitle, string newInfo, string newStatus)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE Subjects SET title = @title, subject_info = @info, status = @status::subject_status WHERE id = @subjectId", 
            connection);

        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@title", newTitle);
        command.Parameters.AddWithValue("@info", newInfo);
        command.Parameters.AddWithValue("@status", newStatus);

        command.ExecuteNonQuery();
    }

    public void DeleteSubject(int subjectId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM Subjects WHERE id = @subjectId", connection);
        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.ExecuteNonQuery();
    }
}
