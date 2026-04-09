create table [Content].[PostReaction] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PostId] uniqueidentifier not null,
    [ReactionId] uniqueidentifier not null,
    constraint [PK_Content_PostReaction] primary key clustered ([Id]),
    constraint [FK_Content_PostReaction_Post] foreign key ([PostId]) references [Content].[Post] ([Id]),
    constraint [FK_Content_PostReaction_Reaction] foreign key ([ReactionId]) references [Content].[Reaction] ([Id]),
);