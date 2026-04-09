create table [Content].[CommentReaction] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CommentId] uniqueidentifier not null,
    [ReactionId] uniqueidentifier not null,
    constraint [PK_Content_CommentReaction] primary key clustered ([Id]),
    constraint [FK_Content_CommentReaction_Comment] foreign key ([CommentId]) references [Content].[Comment] ([Id]),
    constraint [FK_Content_CommentReaction_Reaction] foreign key ([ReactionId]) references [Content].[Reaction] ([Id]),
);