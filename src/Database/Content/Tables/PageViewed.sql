create table [Content].[PageViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PageId] uniqueidentifier not null,
    constraint [PK_Content_PageViewed] primary key clustered ([Id]),
    constraint [FK_Content_PageViewed_Page] foreign key ([PageId]) references [Content].[Page] ([Id]),
);