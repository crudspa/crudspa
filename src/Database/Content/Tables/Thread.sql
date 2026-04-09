create table [Content].[Thread] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ForumId] uniqueidentifier not null,
    [Title] nvarchar(150) not null,
    [CommentId] uniqueidentifier not null,
    [Pinned] bit default(0) not null,
    constraint [PK_Content_Thread] primary key clustered ([Id]),
    constraint [FK_Content_Thread_Forum] foreign key ([ForumId]) references [Content].[Forum] ([Id]),
    constraint [FK_Content_Thread_Comment] foreign key ([CommentId]) references [Content].[Comment] ([Id]),
);