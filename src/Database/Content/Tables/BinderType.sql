create table [Content].[BinderType] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [DesignView] nvarchar(150) null,
    [DisplayView] nvarchar(150) not null,
    [Ordinal] int not null,
    constraint [PK_Content_BinderType] primary key clustered ([Id]),
);