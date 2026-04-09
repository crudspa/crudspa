create table [Content].[JustifyContent] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Content_JustifyContent] primary key clustered ([Id]),
);