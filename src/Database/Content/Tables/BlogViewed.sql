create table [Content].[BlogViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BlogId] uniqueidentifier not null,
    constraint [PK_Content_BlogViewed] primary key clustered ([Id]),
    constraint [FK_Content_BlogViewed_Blog] foreign key ([BlogId]) references [Content].[Blog] ([Id]),
);