using Npgsql;

namespace Klava.DataSeeder.Data;

public class DbManager
{
    private readonly string _connectionString;

    public DbManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public void ClearAllTables()
    {
        using var connection = GetConnection();
        connection.Open();

        var tables = new[]
        {
            "SubjectEvents",
            "SubjectAnnouncements",
            "Files",
            "Submissions",
            "Tasks",
            "Subjects",
            "TeamMembers",
            "Teams",
            "Users"
        };

        foreach (var table in tables)
        {
            using var command = new NpgsqlCommand($"DELETE FROM {table}", connection);
            command.ExecuteNonQuery();
        }

        var sequences = new[]
        {
            "users_id_seq",
            "teams_id_seq",
            "subjects_id_seq",
            "tasks_id_seq",
            "submissions_id_seq",
            "files_id_seq",
            "subjectannouncements_id_seq",
            "subjectevents_id_seq"
        };

        foreach (var sequence in sequences)
        {
            using var command = new NpgsqlCommand($"ALTER SEQUENCE {sequence} RESTART WITH 1", connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
            }
        }
    }
}
