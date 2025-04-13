CREATE TABLE [dbo].[Files_T] (
    [FileID]          INT            IDENTITY (1, 1) NOT NULL,
    [FilePath]        NVARCHAR (400) NOT NULL,
    [FileExtension]   NVARCHAR (10)  NULL,
    [UserID]          INT            NOT NULL,
    [FilePublicTitle] NVARCHAR (400) NULL,
    [FileSafeTitle]   NVARCHAR (400) NULL,
    [SavedFileName]   NVARCHAR (300) NULL,
    [FileType]        INT            NULL,
    [FileStatus]      INT            NULL,
    CONSTRAINT [PK_Files_T] PRIMARY KEY CLUSTERED ([FileID] ASC)
);



