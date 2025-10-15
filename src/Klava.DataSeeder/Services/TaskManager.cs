using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class TaskManager
{
    private readonly DbManager _dbManager;

    public TaskManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateTask(int subjectId, string name, string description, DateTime? deadline)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Tasks (subject_id, name, description, deadline) VALUES (@subjectId, @name, @description, @deadline)", 
            connection);

        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@deadline", deadline ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }

    public List<TaskItem> GetTasksForSubject(int subjectId)
    {
        var tasks = new List<TaskItem>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetTasksForSubject(@subjectId)", connection);
        command.Parameters.AddWithValue("subjectId", subjectId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tasks.Add(new TaskItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Deadline = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
            });
        }

        return tasks;
    }

    public void UpdateTask(int taskId, string newName, string newDescription, DateTime? newDeadline)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE Tasks SET name = @name, description = @description, deadline = @deadline WHERE id = @taskId", 
            connection);

        command.Parameters.AddWithValue("@taskId", taskId);
        command.Parameters.AddWithValue("@name", newName);
        command.Parameters.AddWithValue("@description", newDescription);
        command.Parameters.AddWithValue("@deadline", newDeadline ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }

    public void DeleteTask(int taskId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM Tasks WHERE id = @taskId", connection);
        command.Parameters.AddWithValue("@taskId", taskId);
        command.ExecuteNonQuery();
    }
}
