create table [Education].[ChapterViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ChapterId] uniqueidentifier not null,
    constraint [PK_Education_ChapterViewed] primary key clustered ([Id]),
    constraint [FK_Education_ChapterViewed_Chapter] foreign key ([ChapterId]) references [Education].[Chapter] ([Id]),
);