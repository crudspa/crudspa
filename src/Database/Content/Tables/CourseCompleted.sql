create table [Content].[CourseCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [CourseId] uniqueidentifier not null,
    [Completed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Content_CourseCompleted] primary key clustered ([Id]),
    constraint [FK_Content_CourseCompleted_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_CourseCompleted_Course] foreign key ([CourseId]) references [Content].[Course] ([Id]),
);