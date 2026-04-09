create table [Education].[VocabPartCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssignmentId] uniqueidentifier not null,
    [VocabPartId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_VocabPartCompleted] primary key clustered ([Id]),
    constraint [FK_Education_VocabPartCompleted_Assignment] foreign key ([AssignmentId]) references [Education].[AssessmentAssignment] ([Id]),
    constraint [FK_Education_VocabPartCompleted_VocabPart] foreign key ([VocabPartId]) references [Education].[VocabPart] ([Id]),
);