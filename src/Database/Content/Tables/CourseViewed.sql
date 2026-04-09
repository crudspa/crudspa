create table [Content].[CourseViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CourseId] uniqueidentifier not null,
    constraint [PK_Content_CourseViewed] primary key clustered ([Id]),
    constraint [FK_Content_CourseViewed_Course] foreign key ([CourseId]) references [Content].[Course] ([Id]),
);