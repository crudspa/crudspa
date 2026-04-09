create table [Education].[BookSeason] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Education_BookSeason] primary key clustered ([Id]),
);