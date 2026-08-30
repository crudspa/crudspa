create table [Framework].[AuthConnection] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    [Provider] nvarchar(75) not null,
    [Tenant] nvarchar(255) null,
    [Enabled] bit not null,
    constraint [PK_Framework_AuthConnection] primary key clustered ([Id]),
    constraint [FK_Framework_AuthConnection_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [CK_Framework_AuthConnection_Provider] check (len(ltrim(rtrim([Provider]))) > 0),
    constraint [CK_Framework_AuthConnection_Tenant] check ([Tenant] is null or len(ltrim(rtrim([Tenant]))) > 0),
);

go

create nonclustered index [IX_Framework_AuthConnection_OrganizationProvider]
    on [Framework].[AuthConnection] ([OrganizationId], [Provider], [IsDeleted], [VersionOf]);