create table [Education].[ListenQuestionCategory] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Education_ListenQuestionCategory] primary key clustered ([Id]),
);