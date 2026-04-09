create table [Education].[ObjectiveCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [ObjectiveId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_ObjectiveCompleted] primary key clustered ([Id]),
    constraint [FK_Education_ObjectiveCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_ObjectiveCompleted_Objective] foreign key ([ObjectiveId]) references [Education].[Objective] ([Id]),
);