create table [Content].[Reaction] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ById] uniqueidentifier not null,
    [Reacted] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Emoji] nvarchar(2) null,
    constraint [PK_Content_Reaction] primary key clustered ([Id]),
    constraint [FK_Content_Reaction_By] foreign key ([ById]) references [Framework].[Contact] ([Id]),
);