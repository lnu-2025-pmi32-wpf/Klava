using System.Data;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Models;
using Npgsql;

namespace Klava.DataSeeder.Services;

public class AnnouncementManager
{
    private readonly DbManager _dbManager;

    public AnnouncementManager(DbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public void CreateAnnouncement(int subjectId, string title, string content)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO SubjectAnnouncements (subject_id, title, content) VALUES (@subjectId, @title, @content)", 
            connection);

        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@content", content);

        command.ExecuteNonQuery();
    }

    public List<Announcement> GetAnnouncementsForSubject(int subjectId)
    {
        var announcements = new List<Announcement>();
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM GetAnnouncementsForSubject(@subjectId)", connection);
        command.Parameters.AddWithValue("subjectId", subjectId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            announcements.Add(new Announcement
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2)
            });
        }

        return announcements;
    }

    public void DeleteAnnouncement(int announcementId)
    {
        using var connection = _dbManager.GetConnection();
        connection.Open();

        using var command = new NpgsqlCommand(
            "DELETE FROM SubjectAnnouncements WHERE id = @announcementId", 
            connection);

        command.Parameters.AddWithValue("@announcementId", announcementId);
        command.ExecuteNonQuery();
    }
}
