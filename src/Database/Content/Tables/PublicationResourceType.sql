create table [Content].[PublicationResourceType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(75) not null,
    [Ordinal] int not null,
    constraint [PK_Content_PublicationResourceType] primary key clustered ([Id]),
);