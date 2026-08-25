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
    [PopulationId] uniqueidentifier null,
    [OrganizationId] uniqueidentifier null,
    [ActivationScopeId] uniqueidentifier null,
    constraint [PK_Content_Membership] primary key clustered ([Id]),
    constraint [FK_Content_Membership_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Content_Membership_Population] foreign key ([PopulationId]) references [Content].[Population] ([Id]),
    constraint [FK_Content_Membership_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Content_Membership_ActivationScope] foreign key ([ActivationScopeId]) references [Content].[ActivationScope] ([Id]),
);

go

create nonclustered index [IX_Content_Membership_Population_Organization_Scope]
    on [Content].[Membership] ([PopulationId], [OrganizationId], [ActivationScopeId])
    include ([VersionOf], [IsDeleted], [PortalId]);