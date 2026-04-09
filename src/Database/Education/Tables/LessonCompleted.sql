create table [Education].[LessonCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [LessonId] uniqueidentifier not null,
    [Completed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Education_LessonCompleted] primary key clustered ([Id]),
    constraint [FK_Education_LessonCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_LessonCompleted_Lesson] foreign key ([LessonId]) references [Education].[Lesson] ([Id]),
);