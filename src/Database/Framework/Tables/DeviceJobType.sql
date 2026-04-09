create table [Framework].[DeviceJobType] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [DeviceId] uniqueidentifier not null,
    [JobTypeId] uniqueidentifier not null,
    constraint [PK_Framework_DeviceJobType] primary key clustered ([Id]),
    constraint [FK_Framework_DeviceJobType_Device] foreign key ([DeviceId]) references [Framework].[Device] ([Id]),
    constraint [FK_Framework_DeviceJobType_JobType] foreign key ([JobTypeId]) references [Framework].[JobType] ([Id]),
);