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