create table [Framework].[PortalPermission] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [PermissionId] uniqueidentifier not null,
    constraint [PK_Framework_PortalPermission] primary key clustered ([Id]),
    constraint [FK_Framework_PortalPermission_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_PortalPermission_Permission] foreign key ([PermissionId]) references [Framework].[Permission] ([Id]),
);