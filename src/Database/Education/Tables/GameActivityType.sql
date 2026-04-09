create table [Education].[GameActivityType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [Ordinal] int not null,
    constraint [PK_Education_GameActivityType] primary key clustered ([Id]),
);