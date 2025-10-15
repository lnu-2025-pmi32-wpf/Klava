using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class EventManager
{
    private readonly DbManager _dbManager;

    public EventManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateEvent(int subjectId, DateTime date, string name, string location)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO SubjectEvents (subject_id, date, name, location) VALUES (@subjectId, @date, @name, @location)", 
            connection);

        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@date", date);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@location", location);

        command.ExecuteNonQuery();
    }

    public List<Event> GetEventsForSubject(int subjectId)
    {
        var events = new List<Event>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetEventsForSubject(@subjectId)", connection);
        command.Parameters.AddWithValue("subjectId", subjectId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            events.Add(new Event
            {
                Id = reader.GetInt32(0),
                Date = reader.GetDateTime(1),
                Name = reader.GetString(2),
                Location = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
            });
        }

        return events;
    }

    public void DeleteEvent(int eventId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("DELETE FROM SubjectEvents WHERE id = @eventId", connection);
        command.Parameters.AddWithValue("@eventId", eventId);
        command.ExecuteNonQuery();
    }
}
