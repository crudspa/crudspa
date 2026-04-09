create table [Education].[GoalSettingGroup] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(25) not null,
    [Ordinal] int not null,
    constraint [PK_Education_GoalSettingGroup] primary key clustered ([Id]),
);