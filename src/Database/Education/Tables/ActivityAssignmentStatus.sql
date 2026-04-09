create table [Education].[ActivityAssignmentStatus] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(25) not null,
    [Ordinal] int not null,
    constraint [PK_Education_ActivityAssignmentStatus] primary key clustered ([Id]),
);