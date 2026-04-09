create table [Content].[TagBundle] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [TagId] uniqueidentifier not null,
    [BundleId] uniqueidentifier not null,
    constraint [PK_Content_TagBundle] primary key clustered ([Id]),
    constraint [FK_Content_TagBundle_Tag] foreign key ([TagId]) references [Content].[Tag] ([Id]),
    constraint [FK_Content_TagBundle_Bundle] foreign key ([BundleId]) references [Content].[Bundle] ([Id]),
);