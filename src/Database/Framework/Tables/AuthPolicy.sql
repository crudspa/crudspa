create table [Framework].[AuthPolicy] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    [AuthConnectionId] uniqueidentifier not null,
    [Audience] nvarchar(25) not null,
    [Key] nvarchar(75) null,
    [IdleTimeoutMinutes] int not null,
    [AbsoluteTimeoutMinutes] int not null,
    [Persist] bit not null,
    [AutoRedirect] bit not null,
    [Fallback] bit not null,
    [Enabled] bit not null,
    constraint [PK_Framework_AuthPolicy] primary key clustered ([Id]),
    constraint [FK_Framework_AuthPolicy_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Framework_AuthPolicy_AuthConnection] foreign key ([AuthConnectionId]) references [Framework].[AuthConnection] ([Id]),
    constraint [CK_Framework_AuthPolicy_Audience] check (len(ltrim(rtrim([Audience]))) > 0 and [Audience] not like N'%[^a-z0-9-]%'),
    constraint [CK_Framework_AuthPolicy_Key] check ([Key] is null or ([Audience] = N'student' and [Key] not like N'%[^a-z0-9-]%' and len([Key]) > 0)),
    constraint [CK_Framework_AuthPolicy_Timeouts] check ([IdleTimeoutMinutes] > 0 and [AbsoluteTimeoutMinutes] >= [IdleTimeoutMinutes]),
);

go

create nonclustered index [IX_Framework_AuthPolicy_OrganizationAudience]
    on [Framework].[AuthPolicy] ([OrganizationId], [Audience], [IsDeleted], [VersionOf]);