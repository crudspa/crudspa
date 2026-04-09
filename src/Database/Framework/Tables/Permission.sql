create table [Framework].[Permission] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(100) not null,
    constraint [PK_Framework_Permission] primary key clustered ([Id]),
);