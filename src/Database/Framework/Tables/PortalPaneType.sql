create table [Framework].[PortalPaneType] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [TypeId] uniqueidentifier not null,
    constraint [PK_Framework_PortalPaneType] primary key clustered ([Id]),
    constraint [FK_Framework_PortalPaneType_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_PortalPaneType_Type] foreign key ([TypeId]) references [Framework].[PaneType] ([Id]),
);