create table [Framework].[JobType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [EditorView] nvarchar(250) not null,
    [ActionClass] nvarchar(250) not null,
    constraint [PK_Framework_JobType] primary key clustered ([Id]),
);