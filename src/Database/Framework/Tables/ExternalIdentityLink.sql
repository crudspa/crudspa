create table [Framework].[ExternalIdentityLink] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ExternalIdentityId] uniqueidentifier not null,
    [UserId] uniqueidentifier not null,
    [Method] nvarchar(25) not null,
    [Approved] datetimeoffset(7) not null,
    [ApprovedById] uniqueidentifier null,
    constraint [PK_Framework_ExternalIdentityLink] primary key clustered ([Id]),
    constraint [FK_Framework_ExternalIdentityLink_ExternalIdentity] foreign key ([ExternalIdentityId]) references [Framework].[ExternalIdentity] ([Id]),
    constraint [FK_Framework_ExternalIdentityLink_User] foreign key ([UserId]) references [Framework].[User] ([Id]),
    constraint [FK_Framework_ExternalIdentityLink_ApprovedBy] foreign key ([ApprovedById]) references [Framework].[Session] ([Id]),
    constraint [CK_Framework_ExternalIdentityLink_Method] check (len([Method]) > 0),
);

go

create unique nonclustered index [UX_Framework_ExternalIdentityLink_IdentityUser]
    on [Framework].[ExternalIdentityLink] ([ExternalIdentityId], [UserId])
    where [IsDeleted] = 0;