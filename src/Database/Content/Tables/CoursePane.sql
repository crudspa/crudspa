create table [Content].[CoursePane] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PaneId] uniqueidentifier not null,
    [IdSource] int default(0) not null,
    [CourseId] uniqueidentifier null,
    constraint [PK_Content_CoursePane] primary key clustered ([Id]),
    constraint [FK_Content_CoursePane_Pane] foreign key ([PaneId]) references [Framework].[Pane] ([Id]),
    constraint [FK_Content_CoursePane_Course] foreign key ([CourseId]) references [Content].[Course] ([Id]),
);