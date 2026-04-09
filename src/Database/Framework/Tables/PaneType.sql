create table [Framework].[PaneType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [EditorView] nvarchar(250) null,
    [DisplayView] nvarchar(250) not null,
    constraint [PK_Framework_PaneType] primary key clustered ([Id]),
);