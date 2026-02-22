-- =========================================
-- SCRIPT PARA MICROSERVICIO: Auth.API
-- =========================================
USE MASTER;
GO

CREATE DATABASE SG_Financial_AuthDb;
GO

USE SG_Financial_AuthDb;
GO

CREATE TABLE Users (
   Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
   UserName VARCHAR(50) NOT NULL UNIQUE,
   PasswordHash VARCHAR(255) NOT NULL,
   CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
   IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_Users_UserName ON Users(UserName);
GO