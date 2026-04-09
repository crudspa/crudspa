create table [Education].[BodyTemplate] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [Template] nvarchar(max) not null,
    [Ordinal] int not null,
    constraint [PK_Education_BodyTemplate] primary key clustered ([Id]),
);