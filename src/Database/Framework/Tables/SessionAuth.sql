create table [Framework].[SessionAuth] (
    [Id] uniqueidentifier not null,
    [SessionId] uniqueidentifier not null,
    [ExternalIdentityId] uniqueidentifier null,
    [Provider] nvarchar(75) not null,
    [Authenticated] datetimeoffset(7) not null,
    [LastActivity] datetimeoffset(7) not null,
    [IdleTimeoutMinutes] int not null,
    [IdleExpires] datetimeoffset(7) not null,
    [AbsoluteExpires] datetimeoffset(7) not null,
    [Revoked] datetimeoffset(7) null,
    [RevocationReason] nvarchar(75) null,
    [AuthPolicyId] uniqueidentifier not null,
    constraint [PK_Framework_SessionAuth] primary key clustered ([Id]),
    constraint [FK_Framework_SessionAuth_Session] foreign key ([SessionId]) references [Framework].[Session] ([Id]),
    constraint [FK_Framework_SessionAuth_ExternalIdentity] foreign key ([ExternalIdentityId]) references [Framework].[ExternalIdentity] ([Id]),
    constraint [FK_Framework_SessionAuth_AuthPolicy] foreign key ([AuthPolicyId]) references [Framework].[AuthPolicy] ([Id]),
    constraint [CK_Framework_SessionAuth_Provider] check (len(ltrim(rtrim([Provider]))) > 0),
    constraint [CK_Framework_SessionAuth_IdleTimeoutMinutes] check ([IdleTimeoutMinutes] > 0),
    constraint [CK_Framework_SessionAuth_Activity] check ([LastActivity] >= [Authenticated]),
    constraint [CK_Framework_SessionAuth_IdleExpires] check ([IdleExpires] > [LastActivity] and [IdleExpires] <= [AbsoluteExpires]),
    constraint [CK_Framework_SessionAuth_AbsoluteExpires] check ([AbsoluteExpires] > [Authenticated]),
    constraint [CK_Framework_SessionAuth_Revocation] check (([Revoked] is null and [RevocationReason] is null) or ([Revoked] >= [Authenticated] and len(ltrim(rtrim([RevocationReason]))) > 0)),
);

go

create unique nonclustered index [IX_Framework_SessionAuth_Session]
    on [Framework].[SessionAuth] ([SessionId]);