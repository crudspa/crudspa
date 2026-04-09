create table [Education].[LessonViewed] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [LessonId] uniqueidentifier not null,
    constraint [PK_Education_LessonViewed] primary key clustered ([Id]),
    constraint [FK_Education_LessonViewed_Lesson] foreign key ([LessonId]) references [Education].[Lesson] ([Id]),
);