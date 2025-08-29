CREATE DATABASE TodoList
GO
USE TodoList

CREATE TABLE [User] (
	UserId INT PRIMARY KEY IDENTITY(1, 1),
	[Name] VARCHAR(100) NOT NULL,
	Email VARCHAR(254) UNIQUE NOT NULL,
	HashedPassword VARCHAR(MAX) NOT NULL
)

CREATE TABLE Task (
	TaskId INT PRIMARY KEY IDENTITY(1, 1),
	Title VARCHAR(100) NOT NULL,
	[Description] VARCHAR(MAX),
	UserId INT NOT NULL

	FOREIGN KEY (UserId) REFERENCES [User](UserId),
)
GO

CREATE OR ALTER PROCEDURE sp_CheckIfEmailExists(@Email VARCHAR(254))
AS
BEGIN
	DECLARE @Result BIT

	IF EXISTS (SELECT * FROM [User] WHERE Email = @Email)
		SET @Result = 1
	ELSE
		SET @Result = 0

	RETURN @Result
END
GO

CREATE OR ALTER PROCEDURE sp_InsertAndSelectUser(
	@Name VARCHAR(100),
	@Email VARCHAR(254),
	@HashedPassword VARCHAR(MAX)
) 
AS
BEGIN
	DECLARE @EmailExists BIT
	EXEC @EmailExists = sp_CheckIfEmailExists @Email

	IF @EmailExists = 0
	BEGIN
		INSERT INTO [User] ([Name], Email, HashedPassword)
		VALUES (@Name, @Email, @HashedPassword)

		SELECT * FROM [User] WHERE Email = @Email
	END
END
GO
