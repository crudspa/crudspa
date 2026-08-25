create table [Education].[GameCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [BookId] uniqueidentifier null,
    [GameId] uniqueidentifier null,
    [GameRunId] uniqueidentifier null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_GameCompleted] primary key clustered ([Id]),
    constraint [FK_Education_GameCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_GameCompleted_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
    constraint [FK_Education_GameCompleted_Game] foreign key ([GameId]) references [Education].[Game] ([Id]),
    constraint [FK_Education_GameCompleted_GameRun] foreign key ([GameRunId]) references [Education].[AssignmentBatch] ([Id]),
);

go

create nonclustered index [IX_Education_GameCompleted_UpdatedStudent]
    on [Education].[GameCompleted] ([Updated], [StudentId])
    where [IsDeleted] = 0;

go

create nonclustered index [IX_Education_GameCompleted_StudentUpdated]
    on [Education].[GameCompleted] ([StudentId], [Updated])
    where [IsDeleted] = 0;