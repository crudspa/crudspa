create table [Framework].[PortalFeature] (
    [Id] uniqueidentifier not null,
    [PortalId] uniqueidentifier not null,
    [Key] nvarchar(100) not null,
    [Title] nvarchar(150) not null,
    [IconId] uniqueidentifier not null,
    [PermissionId] uniqueidentifier null,
    constraint [PK_Framework_PortalFeature] primary key clustered ([Id]),
    constraint [FK_Framework_PortalFeature_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_PortalFeature_Icon] foreign key ([IconId]) references [Framework].[Icon] ([Id]),
    constraint [FK_Framework_PortalFeature_Permission] foreign key ([PermissionId]) references [Framework].[Permission] ([Id]),
);