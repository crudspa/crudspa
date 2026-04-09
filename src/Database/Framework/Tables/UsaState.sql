create table [Framework].[UsaState] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Abbreviation] nvarchar(2) not null,
    [Name] nvarchar(50) not null,
    constraint [PK_Framework_UsaState] primary key clustered ([Id]),
);