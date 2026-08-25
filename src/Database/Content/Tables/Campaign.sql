create table [Content].[Campaign] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PortalId] uniqueidentifier not null,
    [Name] nvarchar(75) not null,
    [Description] nvarchar(max) null,
    constraint [PK_Content_Campaign] primary key clustered ([Id]),
    constraint [FK_Content_Campaign_Portal] foreign key ([PortalId]) references [Framework].[Portal] ([Id])
);

go

create nonclustered index [IX_Content_Campaign_Portal]
    on [Content].[Campaign] ([PortalId])
    include ([Id], [VersionOf], [IsDeleted], [Name]);