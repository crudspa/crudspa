create table [Framework].[UserRole] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UserId] uniqueidentifier not null,
    [RoleId] uniqueidentifier not null,
    constraint [PK_Framework_UserRole] primary key clustered ([Id]),
    constraint [FK_Framework_UserRole_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
    constraint [FK_Framework_UserRole_Role] foreign key ([RoleId]) references [Framework].[Role] ([Id]),
);