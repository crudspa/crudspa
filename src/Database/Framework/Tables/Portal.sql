create table [Framework].[Portal] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Key] nvarchar(75) not null,
    [Title] nvarchar(75) not null,
    [OwnerId] uniqueidentifier not null,
    [NavigationTypeId] uniqueidentifier not null,
    [SessionsPersist] bit not null,
    [AllowSignIn] bit not null,
    [RequireSignIn] bit not null,
    constraint [PK_Framework_Portal] primary key clustered ([Id]),
    constraint [FK_Framework_Portal_Owner] foreign key ([OwnerId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Framework_Portal_NavigationType] foreign key ([NavigationTypeId]) references [Framework].[NavigationType] ([Id]),
);