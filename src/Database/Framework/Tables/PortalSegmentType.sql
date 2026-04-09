create table [Framework].[PortalSegmentType] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [TypeId] uniqueidentifier not null,
    constraint [PK_Framework_PortalSegmentType] primary key clustered ([Id]),
    constraint [FK_Framework_PortalSegmentType_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id]),
    constraint [FK_Framework_PortalSegmentType_Type] foreign key ([TypeId]) references [Framework].[SegmentType] ([Id]),
);