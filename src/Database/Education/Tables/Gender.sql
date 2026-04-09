create table [Education].[Gender] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(25) not null,
    [EnglishSubjectivePronoun] nvarchar(25) not null,
    [SpanishSubjectivePronoun] nvarchar(25) not null,
    [EnglishObjectivePronoun] nvarchar(25) not null,
    [SpanishObjectivePronoun] nvarchar(25) not null,
    [EnglishPossessivePronoun] nvarchar(25) not null,
    [SpanishPossessivePronoun] nvarchar(25) not null,
    [EnglishPossessiveAdjective] nvarchar(25) not null,
    [SpanishPossessiveAdjective] nvarchar(25) not null,
    constraint [PK_Education_Gender] primary key clustered ([Id]),
);