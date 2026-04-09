create table [Education].[ReadQuestionType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [Ordinal] int not null,
    constraint [PK_Education_ReadQuestionType] primary key clustered ([Id]),
);