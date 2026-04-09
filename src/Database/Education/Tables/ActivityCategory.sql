create table [Education].[ActivityCategory] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Key] nvarchar(100) not null,
    [Name] nvarchar(100) not null,
    constraint [PK_Education_ActivityCategory] primary key clustered ([Id]),
);