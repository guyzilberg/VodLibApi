CREATE TABLE [dbo].[Users_T] (
    [UserID]    INT            IDENTITY (1, 1) NOT NULL,
    [Username]  NVARCHAR (50)  NOT NULL,
    [password]  NVARCHAR (512) NULL,
    [UserRoles] INT            NULL,
    [PasswordSalt] NVARCHAR(512) NULL, 
    CONSTRAINT [PK_Users_T] PRIMARY KEY CLUSTERED ([Username])
);

