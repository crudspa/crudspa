create table [Framework].[Session] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Started] datetimeoffset(7) not null,
    [Ended] datetimeoffset(7) null,
    [PortalId] uniqueidentifier not null,
    [UserId] uniqueidentifier null,
    [UserAdded] datetimeoffset(7) null,
    constraint [PK_Framework_Session] primary key clustered ([Id]),
    constraint [FK_Framework_Session_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_Session_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
);