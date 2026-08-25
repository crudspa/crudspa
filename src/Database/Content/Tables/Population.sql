create table [Content].[Population] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [Key] nvarchar(75) not null,
    [Name] nvarchar(75) not null,
    [Description] nvarchar(max) null,
    [SupportsOptOut] bit default(0) not null,
    [ResolverKey] nvarchar(150) not null,
    constraint [PK_Content_Population] primary key clustered ([Id]),
    constraint [FK_Content_Population_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
);

go

create unique nonclustered index [IX_Content_Population_Portal_Key]
    on [Content].[Population] ([PortalId], [Key])
    include ([Id], [IsDeleted], [Name], [ResolverKey]);