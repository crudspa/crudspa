create table [Content].[ActivationScope] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ActivationId] uniqueidentifier not null,
    [ParentId] uniqueidentifier null,
    [OrganizationId] uniqueidentifier not null,
    [Name] nvarchar(150) not null,
    [Start] date not null,
    [StartOverridden] bit default(0) not null,
    [Ordinal] int not null,
    constraint [PK_Content_ActivationScope] primary key clustered ([Id]),
    constraint [FK_Content_ActivationScope_Activation] foreign key ([ActivationId]) references [Content].[Activation] ([Id]),
    constraint [FK_Content_ActivationScope_Parent] foreign key ([ParentId]) references [Content].[ActivationScope] ([Id]),
    constraint [FK_Content_ActivationScope_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
);

go

create nonclustered index [IX_Content_ActivationScope_Activation]
    on [Content].[ActivationScope] ([ActivationId], [ParentId], [Ordinal])
    include ([VersionOf], [IsDeleted], [OrganizationId], [Start]);

go

create nonclustered index [IX_Content_ActivationScope_Organization]
    on [Content].[ActivationScope] ([OrganizationId], [ActivationId])
    include ([VersionOf], [IsDeleted], [ParentId]);