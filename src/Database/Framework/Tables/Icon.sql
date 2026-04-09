create table [Framework].[Icon] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [CssClass] nvarchar(50) not null,
    constraint [PK_Framework_Icon] primary key clustered ([Id]),
);