create table [Education].[AssessmentLevel] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Key] nvarchar(100) not null,
    [Name] nvarchar(100) not null,
    [Ordinal] int not null,
    constraint [PK_Education_AssessmentLevel] primary key clustered ([Id]),
);