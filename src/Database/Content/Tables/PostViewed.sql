create table [Content].[PostViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PostId] uniqueidentifier not null,
    constraint [PK_Content_PostViewed] primary key clustered ([Id]),
    constraint [FK_Content_PostViewed_Post] foreign key ([PostId]) references [Content].[Post] ([Id]),
);