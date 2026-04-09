create table [Content].[Membership] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [Name] nvarchar(75) not null,
    [Description] nvarchar(max) null,
    [SupportsOptOut] bit default(0) not null,
    constraint [PK_Content_Membership] primary key clustered ([Id]),
    constraint [FK_Content_Membership_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
);