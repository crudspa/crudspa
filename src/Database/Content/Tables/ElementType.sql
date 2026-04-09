create table [Content].[ElementType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [IconId] uniqueidentifier not null,
    [EditorView] nvarchar(150) not null,
    [DisplayView] nvarchar(150) not null,
    [RepositoryClass] nvarchar(150) not null,
    [OnlyChild] bit default(0) not null,
    [SupportsInteraction] bit default(0) not null,
    [Ordinal] int not null,
    constraint [PK_Content_ElementType] primary key clustered ([Id]),
    constraint [FK_Content_ElementType_Icon] foreign key ([IconId]) references [Framework].[Icon] ([Id]),
);