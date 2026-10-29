CREATE TABLE [dbo].[Todos] (
    [TodoId]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      VARCHAR (MAX)  NOT NULL,
    [IsComplete]       BIT            NOT NULL,
    [DatetimeReminder] DATETIME       NULL,
    [CategoryId]       INT            NOT NULL,
    [UserId]           INT            NOT NULL,
    [DateCreated]      DATETIME       NOT NULL,
    [DateModified]     DATETIME       NOT NULL,
    [IsArchived]       BIT            NOT NULL,
    [DateArchived]     DATETIME       NULL,
    CONSTRAINT [PK_Todos] PRIMARY KEY CLUSTERED ([TodoId] ASC),
    CONSTRAINT [FK_Todos_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]) ON DELETE CASCADE
);
