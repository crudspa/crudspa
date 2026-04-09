create table [Education].[PostReaction] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PostId] uniqueidentifier not null,
    [ById] uniqueidentifier not null,
    [Character] nvarchar(2) null,
    [Reacted] datetimeoffset(7) not null,
    constraint [PK_Education_PostReaction] primary key clustered ([Id]),
    constraint [FK_Education_PostReaction_Post] foreign key ([PostId]) references [Education].[Post] ([Id]),
    constraint [FK_Education_PostReaction_By] foreign key ([ById]) references [Framework].[Contact] ([Id]),
);