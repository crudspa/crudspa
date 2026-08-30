create table [Framework].[AuthHandoff] (
    [Id] uniqueidentifier not null,
    [AuthTransactionId] uniqueidentifier not null,
    [CodeHash] binary(32) not null,
    [PortalId] uniqueidentifier not null,
    [UserId] uniqueidentifier not null,
    [ExternalIdentityId] uniqueidentifier not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Expires] datetimeoffset(7) not null,
    [Consumed] datetimeoffset(7) null,
    [AuthPolicyId] uniqueidentifier not null,
    constraint [PK_Framework_AuthHandoff] primary key clustered ([Id]),
    constraint [FK_Framework_AuthHandoff_AuthTransaction] foreign key ([AuthTransactionId]) references [Framework].[AuthTransaction] ([Id]),
    constraint [FK_Framework_AuthHandoff_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_AuthHandoff_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
    constraint [FK_Framework_AuthHandoff_ExternalIdentity] foreign key ([ExternalIdentityId]) references [Framework].[ExternalIdentity] ([Id]),
    constraint [FK_Framework_AuthHandoff_AuthPolicy] foreign key ([AuthPolicyId]) references [Framework].[AuthPolicy] ([Id]),
    constraint [CK_Framework_AuthHandoff_Expires] check ([Expires] > [Created] and [Expires] <= dateadd(second, 60, [Created])),
    constraint [CK_Framework_AuthHandoff_Consumed] check ([Consumed] is null or [Consumed] >= [Created]),
);

go

create unique nonclustered index [UX_Framework_AuthHandoff_Transaction]
    on [Framework].[AuthHandoff] ([AuthTransactionId]);

go

create unique nonclustered index [UX_Framework_AuthHandoff_CodeHash]
    on [Framework].[AuthHandoff] ([CodeHash]);

go

create nonclustered index [IX_Framework_AuthHandoff_Expires]
    on [Framework].[AuthHandoff] ([Expires])
    where [Consumed] is null;