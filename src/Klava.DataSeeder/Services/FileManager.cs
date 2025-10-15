using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class SubjectFileManager
{
    private readonly DbManager _dbManager;

    public SubjectFileManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateFile(int subjectId, string description, string filePath)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO Files (subject_id, description, file_path) VALUES (@subjectId, @description, @filePath)", 
            connection);

        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@filePath", filePath);

        command.ExecuteNonQuery();
    }

    public List<FileItem> GetFilesForSubject(int subjectId)
    {
        var files = new List<FileItem>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetFilesForSubject(@subjectId)", connection);
        command.Parameters.AddWithValue("subjectId", subjectId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            files.Add(new FileItem
            {
                Id = reader.GetInt32(0),
                Description = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                FilePath = reader.GetString(2)
            });
        }

        return files;
    }

    public void DeleteFile(int fileId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM Files WHERE id = @fileId", connection);
        command.Parameters.AddWithValue("@fileId", fileId);
        command.ExecuteNonQuery();
    }
}
