using Klava.DataSeeder.Services;

namespace Klava.DataSeeder;

public class SeedData
{
    private readonly UserManager _userManager;
    private readonly TeamManager _teamManager;
    private readonly TeamMemberManager _teamMemberManager;
    private readonly SubjectManager _subjectManager;
    private readonly TaskManager _taskManager;
    private readonly SubmissionManager _submissionManager;
    private readonly SubjectFileManager _fileManager;
    private readonly AnnouncementManager _announcementManager;
    private readonly EventManager _eventManager;
    private readonly Random _random = new();

    public SeedData(
        UserManager userManager,
        TeamManager teamManager,
        TeamMemberManager teamMemberManager,
        SubjectManager subjectManager,
        TaskManager taskManager,
        SubmissionManager submissionManager,
        SubjectFileManager fileManager,
        AnnouncementManager announcementManager,
        EventManager eventManager)
    {
        _userManager = userManager;
        _teamManager = teamManager;
        _teamMemberManager = teamMemberManager;
        _subjectManager = subjectManager;
        _taskManager = taskManager;
        _submissionManager = submissionManager;
        _fileManager = fileManager;
        _announcementManager = announcementManager;
        _eventManager = eventManager;
    }

    public void SeedDatabase()
    {
        Console.WriteLine("Starting database seeding...");

        Console.WriteLine("Creating users...");
        CreateUsers(50);

        Console.WriteLine("Creating teams...");
        CreateTeams(30);

        Console.WriteLine("Adding team members...");
        AddTeamMembers(40);

        Console.WriteLine("Creating subjects...");
        CreateSubjects(35);

        Console.WriteLine("Creating tasks...");
        CreateTasks(40);

        Console.WriteLine("Creating submissions...");
        CreateSubmissions(45);

        Console.WriteLine("Creating files...");
        CreateFiles(30);

        Console.WriteLine("Creating announcements...");
        CreateAnnouncements(35);

        Console.WriteLine("Creating events...");
        CreateEvents(30);

        Console.WriteLine("Database seeding completed successfully!");
    }

    private void CreateUsers(int count)
    {
        var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emma", "Robert", "Lisa", "William", "Jennifer", 
            "James", "Mary", "Richard", "Patricia", "Charles", "Linda", "Thomas", "Barbara", "Christopher", "Elizabeth",
            "Daniel", "Jessica", "Matthew", "Susan", "Anthony", "Karen", "Mark", "Nancy", "Donald", "Betty" };
        
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson" };

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[_random.Next(firstNames.Length)];
            var lastName = lastNames[_random.Next(lastNames.Length)];
            var password = $"password{i + 1}";

            _userManager.CreateUser(firstName, lastName, password);
        }
    }

    private void CreateTeams(int count)
    {
        var teamPrefixes = new[] { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa",
            "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma", "Tau", "Upsilon" };
        
        var teamSuffixes = new[] { "Team", "Squad", "Group", "Force", "Unit", "Division", "Brigade", "Legion", "Crew", "Guild" };

        for (int i = 0; i < count; i++)
        {
            var teamName = $"{teamPrefixes[_random.Next(teamPrefixes.Length)]} {teamSuffixes[_random.Next(teamSuffixes.Length)]}";
            var accessCode = GenerateRandomCode(8);
            var ownerId = _random.Next(1, 51);

            _teamManager.CreateTeam(teamName, accessCode, ownerId);
        }
    }

    private void AddTeamMembers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var userId = _random.Next(1, 51);
            var teamId = _random.Next(1, 31);
            var role = _random.Next(10) < 8 ? "student" : "headman";

            try
            {
                _teamMemberManager.AddUserToTeam(userId, teamId, role);
            }
            catch
            {
            }
        }
    }

    private void CreateSubjects(int count)
    {
        var subjectNames = new[] { "Mathematics", "Physics", "Chemistry", "Biology", "Computer Science", "History", 
            "Geography", "Literature", "Art", "Music", "Physical Education", "Economics", "Philosophy", 
            "Psychology", "Sociology", "Engineering", "Architecture", "Law", "Medicine", "Business" };
        
        var levels = new[] { "Introduction to", "Advanced", "Intermediate", "Fundamentals of", "Applied", "Theoretical" };

        for (int i = 0; i < count; i++)
        {
            var teamId = _random.Next(1, 31);
            var title = $"{levels[_random.Next(levels.Length)]} {subjectNames[_random.Next(subjectNames.Length)]}";
            var info = $"This is a comprehensive course covering important topics in {subjectNames[_random.Next(subjectNames.Length)]}.";
            var status = _random.Next(2) == 0 ? "exam" : "test";

            _subjectManager.CreateSubject(teamId, title, info, status);
        }
    }

    private void CreateTasks(int count)
    {
        var taskTypes = new[] { "Homework", "Assignment", "Project", "Lab Work", "Essay", "Presentation", 
            "Quiz Preparation", "Research", "Case Study", "Problem Set" };

        for (int i = 0; i < count; i++)
        {
            var subjectId = _random.Next(1, 36);
            var name = $"{taskTypes[_random.Next(taskTypes.Length)]} #{i + 1}";
            var description = $"Complete the {taskTypes[_random.Next(taskTypes.Length)].ToLower()} according to the guidelines provided.";
            var deadline = DateTime.Now.AddDays(_random.Next(1, 60));

            _taskManager.CreateTask(subjectId, name, description, deadline);
        }
    }

    private void CreateSubmissions(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var taskId = _random.Next(1, 41);
            var userId = _random.Next(1, 51);

            try
            {
                _submissionManager.CreateSubmission(taskId, userId);
                
                if (_random.Next(10) < 6)
                {
                    _submissionManager.UpdateSubmissionStatus(i + 1, "done");
                }
            }
            catch
            {
            }
        }
    }

    private void CreateFiles(int count)
    {
        var fileTypes = new[] { "Lecture Notes", "Slides", "Reference Material", "Textbook", "Study Guide", 
            "Video Recording", "Supplementary Reading", "Practice Problems" };
        
        var extensions = new[] { ".pdf", ".pptx", ".docx", ".mp4", ".txt" };

        for (int i = 0; i < count; i++)
        {
            var subjectId = _random.Next(1, 36);
            var fileType = fileTypes[_random.Next(fileTypes.Length)];
            var description = $"{fileType} for lesson {_random.Next(1, 20)}";
            var filePath = $"/files/subject_{subjectId}/{fileType.Replace(" ", "_").ToLower()}_{i + 1}{extensions[_random.Next(extensions.Length)]}";

            _fileManager.CreateFile(subjectId, description, filePath);
        }
    }

    private void CreateAnnouncements(int count)
    {
        var announcementTitles = new[] { "Class Canceled", "Exam Date Changed", "New Material Available", 
            "Office Hours Update", "Guest Lecture", "Deadline Extension", "Important Notice", 
            "Course Update", "Assignment Posted", "Grading Complete" };
        
        var contents = new[] { 
            "Please be advised of the following changes to the course schedule.",
            "Important information regarding upcoming assessments.",
            "New resources have been added to the course materials.",
            "Please review this information carefully and reach out if you have questions.",
            "This announcement contains critical information for all students."
        };

        for (int i = 0; i < count; i++)
        {
            var subjectId = _random.Next(1, 36);
            var title = announcementTitles[_random.Next(announcementTitles.Length)];
            var content = contents[_random.Next(contents.Length)];

            _announcementManager.CreateAnnouncement(subjectId, title, content);
        }
    }

    private void CreateEvents(int count)
    {
        var eventTypes = new[] { "Midterm Exam", "Final Exam", "Quiz", "Workshop", "Seminar", 
            "Lab Session", "Review Session", "Guest Lecture", "Group Presentation", "Field Trip" };
        
        var locations = new[] { "Room 101", "Room 202", "Room 303", "Main Hall", "Lab A", 
            "Lab B", "Auditorium", "Online", "Library Room 5", "Conference Room" };

        for (int i = 0; i < count; i++)
        {
            var subjectId = _random.Next(1, 36);
            var eventDate = DateTime.Now.AddDays(_random.Next(1, 90));
            var name = eventTypes[_random.Next(eventTypes.Length)];
            var location = locations[_random.Next(locations.Length)];

            _eventManager.CreateEvent(subjectId, eventDate, name, location);
        }
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
