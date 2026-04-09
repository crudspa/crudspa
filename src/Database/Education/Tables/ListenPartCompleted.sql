create table [Education].[ListenPartCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [ListenPartId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_ListenPartCompleted] primary key clustered ([Id]),
    constraint [FK_Education_ListenPartCompleted_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_ListenPartCompleted_ListenPart] foreign key ([ListenPartId]) references [Education].[ListenPart] ([Id]),
);