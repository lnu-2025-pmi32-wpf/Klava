# Klava Database Manager

A console application for managing the Klava database using PostgreSQL and ADO.NET (Npgsql).

## Project Structure

```
Klava.DataSeeder/
├── Data/
│   └── DbManager.cs              # Database connection manager
├── Models/
│   ├── User.cs                   # User model
│   ├── Team.cs                   # Team model
│   ├── TeamMember.cs             # Team member model
│   ├── Subject.cs                # Subject model
│   ├── Task.cs                   # Task model
│   ├── Submission.cs             # Submission model
│   ├── FileItem.cs               # File model
│   ├── Announcement.cs           # Announcement model
│   └── Event.cs                  # Event model
├── Services/
│   ├── UserManager.cs            # User CRUD operations
│   ├── TeamManager.cs            # Team CRUD operations
│   ├── TeamMemberManager.cs      # Team member operations
│   ├── SubjectManager.cs         # Subject CRUD operations
│   ├── TaskManager.cs            # Task CRUD operations
│   ├── SubmissionManager.cs      # Submission operations
│   ├── SubjectFileManager.cs     # File operations
│   ├── AnnouncementManager.cs    # Announcement operations
│   └── EventManager.cs           # Event operations
├── Config.cs                     # Database configuration
├── SeedData.cs                   # Test data generator
└── Program.cs                    # Main application with console menu
```

## Features

- **Database Connection**: Uses ADO.NET (Npgsql) to connect to PostgreSQL
- **Service Managers**: Separate managers for each database entity
- **Stored Procedures**: All operations use PostgreSQL stored procedures
- **Test Data Generator**: Generates 30-50 records for each table
- **Console Menu**: Interactive menu for database operations

## Configuration

Update the connection string in `Config.cs`:

```csharp
public static string ConnectionString => 
    "Host=localhost;Port=5432;Database=klava_db;Username=postgres;Password=postgres";
```

## Prerequisites

1. .NET 9.0 SDK
2. PostgreSQL database with the schema and stored procedures installed
3. Run the provided SQL script to create tables, types, procedures, and functions

## How to Run

1. Build the project:
   ```
   dotnet build
   ```

2. Run the application:
   ```
   dotnet run
   ```

## Menu Options

1. **Seed Database with Test Data** - Generates and inserts test data (30-50 records per table)
2. **View All Users** - Displays all users in the database
3. **View All Teams** - Displays all teams with their details
4. **Add User** - Create a new user
5. **Update User** - Update an existing user
6. **Delete User** - Delete a user by ID
7. **Add Team** - Create a new team
8. **View Team Members** - View members of a specific team
9. **View Subjects for Team** - View subjects assigned to a team
0. **Exit** - Close the application

## Test Data Generation

The seed data generator creates:
- **50 users** with random first and last names
- **30 teams** with random names and access codes
- **40 team member** assignments
- **35 subjects** with various topics and statuses
- **40 tasks** with deadlines
- **45 submissions** (60% marked as done)
- **30 files** with various types
- **35 announcements** for subjects
- **30 events** with dates and locations

## Architecture

- **DbManager**: Manages database connections
- **Service Managers**: Each manager handles CRUD operations for its entity using stored procedures
- **Models**: Simple POCOs representing database entities
- **SeedData**: Generates realistic test data using randomization
