create table [Content].[PostTag] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PostId] uniqueidentifier not null,
    [TagId] uniqueidentifier not null,
    constraint [PK_Content_PostTag] primary key clustered ([Id]),
    constraint [FK_Content_PostTag_Post] foreign key ([PostId]) references [Content].[Post] ([Id]),
    constraint [FK_Content_PostTag_Tag] foreign key ([TagId]) references [Content].[Tag] ([Id]),
);