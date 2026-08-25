create table [Content].[ForumBundle] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ForumId] uniqueidentifier not null,
    [BundleId] uniqueidentifier not null,
    [ThreadRule] int default(0) not null,
    [CommentRule] int default(0) not null,
    constraint [PK_Content_ForumBundle] primary key clustered ([Id]),
    constraint [FK_Content_ForumBundle_Forum] foreign key ([ForumId]) references [Content].[Forum] ([Id]),
    constraint [FK_Content_ForumBundle_Bundle] foreign key ([BundleId]) references [Content].[Bundle] ([Id]),
    constraint [CK_Content_ForumBundle_ThreadRule] check ([ThreadRule] in (0, 1, 2)),
    constraint [CK_Content_ForumBundle_CommentRule] check ([CommentRule] in (0, 1, 2)),
);

go

create nonclustered index [IX_Content_ForumBundle_ForumId_BundleId]
    on [Content].[ForumBundle] ([ForumId], [BundleId])
    include ([Id], [VersionOf], [IsDeleted], [ThreadRule], [CommentRule]);