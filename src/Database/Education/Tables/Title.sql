create table [Education].[Title] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Education_Title] primary key clustered ([Id]),
);