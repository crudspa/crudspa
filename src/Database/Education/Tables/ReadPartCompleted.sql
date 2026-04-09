create table [Education].[ReadPartCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [ReadPartId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_ReadPartCompleted] primary key clustered ([Id]),
    constraint [FK_Education_ReadPartCompleted_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_ReadPartCompleted_ReadPart] foreign key ([ReadPartId]) references [Education].[ReadPart] ([Id]),
);