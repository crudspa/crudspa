create table [Content].[Direction] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Content_Direction] primary key clustered ([Id]),
);