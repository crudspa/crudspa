create table [Framework].[RolePermission] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [RoleId] uniqueidentifier not null,
    [PermissionId] uniqueidentifier not null,
    constraint [PK_Framework_RolePermission] primary key clustered ([Id]),
    constraint [FK_Framework_RolePermission_Role] foreign key ([RoleId]) references [Framework].[Role] ([Id]),
    constraint [FK_Framework_RolePermission_Permission] foreign key ([PermissionId]) references [Framework].[Permission] ([Id]),
);