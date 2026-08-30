create table [Framework].[AuthEvent] (
    [Id] uniqueidentifier not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [CorrelationId] uniqueidentifier not null,
    [Type] nvarchar(50) not null,
    [Outcome] nvarchar(25) not null,
    [Provider] nvarchar(75) null,
    [Tenant] nvarchar(255) null,
    [Audience] nvarchar(25) null,
    [PortalId] uniqueidentifier null,
    [ExternalIdentityId] uniqueidentifier null,
    [SessionId] uniqueidentifier null,
    [Reason] nvarchar(75) null,
    constraint [PK_Framework_AuthEvent] primary key clustered ([Id]),
    constraint [CK_Framework_AuthEvent_Type] check ([Type] in (N'auth-started', N'auth-completed', N'session-started', N'session-revoked')),
    constraint [CK_Framework_AuthEvent_Outcome] check ([Outcome] in (N'succeeded', N'rejected')),
    constraint [CK_Framework_AuthEvent_Reason] check ([Reason] is null or len(ltrim(rtrim([Reason]))) > 0),
);

go

create nonclustered index [IX_Framework_AuthEvent_CorrelationId]
    on [Framework].[AuthEvent] ([CorrelationId], [Created]);