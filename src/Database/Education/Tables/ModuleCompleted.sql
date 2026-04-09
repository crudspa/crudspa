create table [Education].[ModuleCompleted] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [ModuleId] uniqueidentifier not null,
    [DeviceTimestamp] datetimeoffset(7) not null,
    constraint [PK_Education_ModuleCompleted] primary key clustered ([Id]),
    constraint [FK_Education_ModuleCompleted_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_ModuleCompleted_Module] foreign key ([ModuleId]) references [Education].[Module] ([Id]),
);