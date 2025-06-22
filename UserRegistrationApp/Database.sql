-- Create Database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UserRegistrationDB')
BEGIN
    CREATE DATABASE UserRegistrationDB;
END
GO

USE UserRegistrationDB;
GO

-- Drop existing tables if they exist
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Cities', 'U') IS NOT NULL DROP TABLE dbo.Cities;
IF OBJECT_ID('dbo.States', 'U') IS NOT NULL DROP TABLE dbo.States;
GO

-- Create States table
CREATE TABLE States (
    StateId INT PRIMARY KEY IDENTITY,
    StateName NVARCHAR(100) NOT NULL
);
GO

-- Create Cities table
CREATE TABLE Cities (
    CityId INT PRIMARY KEY IDENTITY,
    CityName NVARCHAR(100) NOT NULL,
    StateId INT NOT NULL,
    FOREIGN KEY (StateId) REFERENCES States(StateId)
);
GO

-- Create Users table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(25) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Email NVARCHAR(255) NULL,
    Mobile NVARCHAR(15) NULL,
    Phone NVARCHAR(15) NULL,
    StateId INT NOT NULL,
    CityId INT NOT NULL,
    Hobbies NVARCHAR(200) NULL,
    PhotoPath NVARCHAR(MAX) NULL,
    TermsAgreed BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Users_States FOREIGN KEY (StateId) REFERENCES States(StateId),
    CONSTRAINT FK_Users_Cities FOREIGN KEY (CityId) REFERENCES Cities(CityId),
    CONSTRAINT CHK_Contact_Info CHECK (Mobile IS NOT NULL OR Phone IS NOT NULL)
);
GO

-- Seed States table
INSERT INTO States (StateName) VALUES ('Delhi'), ('UP');
GO

-- Seed Cities table
-- Delhi Cities
INSERT INTO Cities (CityName, StateId) VALUES ('New Delhi', (SELECT StateId FROM States WHERE StateName = 'Delhi'));
INSERT INTO Cities (CityName, StateId) VALUES ('North Delhi', (SELECT StateId FROM States WHERE StateName = 'Delhi'));

-- UP Cities
INSERT INTO Cities (CityName, StateId) VALUES ('Lucknow', (SELECT StateId FROM States WHERE StateName = 'UP'));
INSERT INTO Cities (CityName, StateId) VALUES ('Kanpur', (SELECT StateId FROM States WHERE StateName = 'UP'));
INSERT INTO Cities (CityName, StateId) VALUES ('Noida', (SELECT StateId FROM States WHERE StateName = 'UP'));
GO

PRINT 'Database schema and initial data created successfully.';
GO 