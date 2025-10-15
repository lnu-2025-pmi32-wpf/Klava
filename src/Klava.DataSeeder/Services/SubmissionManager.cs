using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class SubmissionManager
{
    private readonly DbManager _dbManager;

    public SubmissionManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateSubmission(int taskId, int userId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Submissions (task_id, user_id, status) VALUES (@taskId, @userId, 'wait'::submission_status)", 
            connection);

        command.Parameters.AddWithValue("@taskId", taskId);
        command.Parameters.AddWithValue("@userId", userId);

        command.ExecuteNonQuery();
    }

    public List<Submission> GetSubmissionsForTask(int taskId)
    {
        var submissions = new List<Submission>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetSubmissionsForTask(@taskId)", connection);
        command.Parameters.AddWithValue("taskId", taskId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            submissions.Add(new Submission
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Status = reader.GetString(2).ToLower() == "done" ? SubmissionStatus.Done : SubmissionStatus.Wait
            });
        }

        return submissions;
    }

    public void UpdateSubmissionStatus(int submissionId, string newStatus)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE Submissions SET status = @status::submission_status WHERE id = @submissionId", 
            connection);

        command.Parameters.AddWithValue("@submissionId", submissionId);
        command.Parameters.AddWithValue("@status", newStatus);

        command.ExecuteNonQuery();
    }
}
