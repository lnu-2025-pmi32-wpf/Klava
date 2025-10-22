-- TABLES:
CREATE TABLE Users (
                       id SERIAL PRIMARY KEY,
                       firstname VARCHAR(100) NOT NULL,
                       password VARCHAR(255) NOT NULL,
                       lastname VARCHAR(100) NOT NULL
);

CREATE TABLE Teams (
                       id SERIAL PRIMARY KEY,
                       name VARCHAR(50) NOT NULL,
                       code VARCHAR(8) NOT NULL,
                       owner_id INT,
                       FOREIGN KEY (owner_id) REFERENCES Users(id) ON DELETE SET NULL
);

CREATE TYPE team_member_role AS ENUM ('student', 'headman');

CREATE TABLE TeamMembers (
                             team_id INT NOT NULL,
                             user_id INT NOT NULL,
                             role team_member_role NOT NULL DEFAULT 'student',
                             PRIMARY KEY (team_id, user_id),
                             FOREIGN KEY (team_id) REFERENCES Teams(id) ON DELETE CASCADE,
                             FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE
);

CREATE TYPE subject_status AS ENUM ('exam', 'test');

CREATE TABLE Subjects (
                          id SERIAL PRIMARY KEY,
                          team_id INT NOT NULL,
                          title VARCHAR(255) NOT NULL,
                          subject_info TEXT,
                          status subject_status NOT NULL DEFAULT 'exam',
                          FOREIGN KEY (team_id) REFERENCES Teams(id) ON DELETE CASCADE
);

CREATE TABLE Tasks (
                       id SERIAL PRIMARY KEY,
                       subject_id INT NOT NULL,
                       name VARCHAR(50) NOT NULL,
                       description TEXT,
                       deadline TIMESTAMP,
                       FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

CREATE TABLE Files (
                       id SERIAL PRIMARY KEY,
                       subject_id INT NOT NULL,
                       description VARCHAR(255),
                       file_path VARCHAR(512) NOT NULL,
                       FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

CREATE TYPE submission_status AS ENUM ('done', 'wait');

CREATE TABLE Submissions (
                             id SERIAL PRIMARY KEY,
                             task_id INT NOT NULL,
                             user_id INT NOT NULL,
                             status submission_status NOT NULL DEFAULT 'wait',
                             FOREIGN KEY (task_id) REFERENCES Tasks(id) ON DELETE CASCADE,
                             FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE
);

CREATE TABLE SubjectAnnouncements (
                                      id SERIAL PRIMARY KEY,
                                      subject_id INT NOT NULL,
                                      title VARCHAR(50) NOT NULL,
                                      content TEXT NOT NULL,
                                      FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

CREATE TABLE SubjectEvents (
                               id SERIAL PRIMARY KEY,
                               subject_id INT NOT NULL,
                               date TIMESTAMP NOT NULL,
                               name VARCHAR(100) NOT NULL,
                               location VARCHAR(255),
                               FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

-- FUNCTIONS:

-- Get user by ID
CREATE OR REPLACE FUNCTION GetUserById(p_user_id INT)
RETURNS TABLE (
    id INT,
    firstname VARCHAR(100),
    lastname VARCHAR(100)
) AS $$
BEGIN
    RETURN QUERY
    SELECT u.id, u.firstname, u.lastname
    FROM Users u
    WHERE u.id = p_user_id;
END;
$$ LANGUAGE plpgsql;

-- Get team by ID
CREATE OR REPLACE FUNCTION GetTeamById(p_team_id INT)
RETURNS TABLE (
    id INT,
    name VARCHAR(50),
    code VARCHAR(8),
    owner_id INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT t.id, t.name, t.code, t.owner_id
    FROM Teams t
    WHERE t.id = p_team_id;
END;
$$ LANGUAGE plpgsql;

-- Get teams for user
CREATE OR REPLACE FUNCTION GetTeamsForUser(p_user_id INT)
RETURNS TABLE (
    id INT,
    name VARCHAR(50),
    code VARCHAR(8)
) AS $$
BEGIN
    RETURN QUERY
    SELECT t.id, t.name, t.code
    FROM Teams t
    JOIN TeamMembers tm ON t.id = tm.team_id
    WHERE tm.user_id = p_user_id;
END;
$$ LANGUAGE plpgsql;

-- Get team members
CREATE OR REPLACE FUNCTION GetTeamMembers(p_team_id INT)
RETURNS TABLE (
    id INT,
    firstname VARCHAR(100),
    lastname VARCHAR(100),
    role team_member_role
) AS $$
BEGIN
    RETURN QUERY
    SELECT u.id, u.firstname, u.lastname, tm.role
    FROM Users u
    JOIN TeamMembers tm ON u.id = tm.user_id
    WHERE tm.team_id = p_team_id;
END;
$$ LANGUAGE plpgsql;

-- Get subject by ID
CREATE OR REPLACE FUNCTION GetSubjectById(p_subject_id INT)
RETURNS TABLE (
    id INT,
    title VARCHAR(255),
    subject_info TEXT,
    status subject_status
) AS $$
BEGIN
    RETURN QUERY
    SELECT s.id, s.title, s.subject_info, s.status
    FROM Subjects s
    WHERE s.id = p_subject_id;
END;
$$ LANGUAGE plpgsql;

-- Get subjects for team
CREATE OR REPLACE FUNCTION GetSubjectsForTeam(p_team_id INT)
RETURNS TABLE (
    id INT,
    title VARCHAR(255),
    status subject_status
) AS $$
BEGIN
    RETURN QUERY
    SELECT s.id, s.title, s.status
    FROM Subjects s
    WHERE s.team_id = p_team_id;
END;
$$ LANGUAGE plpgsql;

-- Get tasks for subject
CREATE OR REPLACE FUNCTION GetTasksForSubject(p_subject_id INT)
RETURNS TABLE (
    id INT,
    name VARCHAR(50),
    description TEXT,
    deadline TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT t.id, t.name, t.description, t.deadline
    FROM Tasks t
    WHERE t.subject_id = p_subject_id;
END;
$$ LANGUAGE plpgsql;

-- Get files for subject
CREATE OR REPLACE FUNCTION GetFilesForSubject(p_subject_id INT)
RETURNS TABLE (
    id INT,
    description VARCHAR(255),
    file_path VARCHAR(512)
) AS $$
BEGIN
    RETURN QUERY
    SELECT f.id, f.description, f.file_path
    FROM Files f
    WHERE f.subject_id = p_subject_id;
END;
$$ LANGUAGE plpgsql;

-- Get submissions for task
CREATE OR REPLACE FUNCTION GetSubmissionsForTask(p_task_id INT)
RETURNS TABLE (
    id INT,
    user_id INT,
    status submission_status
) AS $$
BEGIN
    RETURN QUERY
    SELECT s.id, s.user_id, s.status
    FROM Submissions s
    WHERE s.task_id = p_task_id;
END;
$$ LANGUAGE plpgsql;

-- Get announcements for subject
CREATE OR REPLACE FUNCTION GetAnnouncementsForSubject(p_subject_id INT)
RETURNS TABLE (
    id INT,
    title VARCHAR(50),
    content TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT sa.id, sa.title, sa.content
    FROM SubjectAnnouncements sa
    WHERE sa.subject_id = p_subject_id;
END;
$$ LANGUAGE plpgsql;

-- Get events for subject
CREATE OR REPLACE FUNCTION GetEventsForSubject(p_subject_id INT)
RETURNS TABLE (
    id INT,
    date TIMESTAMP,
    name VARCHAR(100),
    location VARCHAR(255)
) AS $$
BEGIN
    RETURN QUERY
    SELECT se.id, se.date, se.name, se.location
    FROM SubjectEvents se
    WHERE se.subject_id = p_subject_id;
END;
$$ LANGUAGE plpgsql;
