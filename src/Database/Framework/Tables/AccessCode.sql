create table [Framework].[AccessCode] (
    [Id] uniqueidentifier not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [SessionId] uniqueidentifier not null,
    [UserId] uniqueidentifier not null,
    [PortalId] uniqueidentifier not null,
    [Code] nvarchar(40) not null,
    [Expires] datetimeoffset(7) not null,
    [Used] datetimeoffset(7) null,
    constraint [PK_Framework_AccessCode] primary key clustered ([Id]),
    constraint [FK_Framework_AccessCode_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
    constraint [FK_Framework_AccessCode_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
);