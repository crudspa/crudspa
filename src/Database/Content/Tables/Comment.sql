create table [Content].[Comment] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ParentId] uniqueidentifier null,
    [PostId] uniqueidentifier null,
    [ThreadId] uniqueidentifier null,
    [ById] uniqueidentifier not null,
    [ByOrganizationName] nvarchar(75) null,
    [Posted] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Edited] datetimeoffset(7) null,
    [Removed] bit default(0) not null,
    [Body] nvarchar(max) not null,
    constraint [PK_Content_Comment] primary key clustered ([Id]),
    constraint [FK_Content_Comment_Parent] foreign key ([ParentId]) references [Content].[Comment] ([Id]),
    constraint [FK_Content_Comment_Post] foreign key ([PostId]) references [Content].[Post] ([Id]),
    constraint [FK_Content_Comment_Thread] foreign key ([ThreadId]) references [Content].[Thread] ([Id]),
    constraint [FK_Content_Comment_By] foreign key ([ById]) references [Framework].[Contact] ([Id]),
);

go

create nonclustered index [IX_Content_Comment_Thread_Posted_Parent]
    on [Content].[Comment] ([ThreadId], [Posted], [ParentId])
    include ([Id], [VersionOf], [IsDeleted], [ById], [ByOrganizationName], [Edited], [Removed]);