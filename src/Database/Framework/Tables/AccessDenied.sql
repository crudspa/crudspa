create table [Framework].[AccessDenied] (
    [Id] uniqueidentifier not null,
    [Denied] datetimeoffset(7) not null,
    [SessionId] uniqueidentifier null,
    [EventType] nvarchar(50) not null,
    [PermissionId] uniqueidentifier null,
    [Method] nvarchar(250) not null,
    constraint [PK_Framework_AccessDenied] primary key clustered ([Id]),
);