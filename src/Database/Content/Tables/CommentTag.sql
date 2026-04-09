create table [Content].[CommentTag] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CommentId] uniqueidentifier not null,
    [TagId] uniqueidentifier not null,
    constraint [PK_Content_CommentTag] primary key clustered ([Id]),
    constraint [FK_Content_CommentTag_Comment] foreign key ([CommentId]) references [Content].[Comment] ([Id]),
    constraint [FK_Content_CommentTag_Tag] foreign key ([TagId]) references [Content].[Tag] ([Id]),
);