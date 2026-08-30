create table [Framework].[ExternalIdentity] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Created] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Provider] nvarchar(75) not null,
    [Issuer] nvarchar(500) not null,
    [Subject] nvarchar(255) not null,
    [Tenant] nvarchar(255) not null,
    [ProviderRole] nvarchar(50) null,
    [Enabled] bit default(1) not null,
    [LastSeen] datetimeoffset(7) not null,
    [KeyHash] binary(32) not null,
    constraint [PK_Framework_ExternalIdentity] primary key clustered ([Id]),
    constraint [CK_Framework_ExternalIdentity_Provider] check (len([Provider]) > 0),
    constraint [CK_Framework_ExternalIdentity_Issuer] check (len([Issuer]) > 0),
    constraint [CK_Framework_ExternalIdentity_Subject] check (len([Subject]) > 0),
    constraint [CK_Framework_ExternalIdentity_Tenant] check (len([Tenant]) > 0),
);

go

create unique nonclustered index [UX_Framework_ExternalIdentity_KeyHash]
    on [Framework].[ExternalIdentity] ([KeyHash])
    where [IsDeleted] = 0;