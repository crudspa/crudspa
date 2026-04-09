create table [Education].[StudentStatus] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(25) not null,
    [Ordinal] int not null,
    constraint [PK_Education_StudentStatus] primary key clustered ([Id]),
);