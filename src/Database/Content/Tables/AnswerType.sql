create table [Content].[AnswerType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [DesignView] nvarchar(150) not null,
    [DisplayView] nvarchar(150) not null,
    constraint [PK_Content_AnswerType] primary key clustered ([Id]),
);