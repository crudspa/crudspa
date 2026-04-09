create table [Content].[RuleType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [EditorView] nvarchar(250) not null,
    [DisplayView] nvarchar(250) not null,
    constraint [PK_Content_RuleType] primary key clustered ([Id]),
);