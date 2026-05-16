create table [Framework].[CostGuardFinding] (
    [Id] uniqueidentifier not null,
    [JobId] uniqueidentifier null,
    [ScheduleId] uniqueidentifier null,
    [ScheduleName] nvarchar(50) null,
    [Occurred] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Severity] int not null,
    [SeverityName] nvarchar(25) not null,
    [Area] nvarchar(75) not null,
    [EnvironmentName] nvarchar(100) not null,
    [ResourceName] nvarchar(255) not null,
    [AlertKey] nvarchar(450) not null,
    [Message] nvarchar(max) not null,
    [ImmediateEmailSent] datetimeoffset(7) null,
    constraint [PK_Framework_CostGuardFinding] primary key clustered ([Id]),
    constraint [FK_Framework_CostGuardFinding_Job] foreign key ([JobId]) references [Framework].[Job] ([Id]),
    constraint [FK_Framework_CostGuardFinding_Schedule] foreign key ([ScheduleId]) references [Framework].[JobSchedule] ([Id]),
);

GO

create nonclustered index [IX_Framework_CostGuardFinding_Occurred]
on [Framework].[CostGuardFinding] ([Occurred] desc);

GO

create nonclustered index [IX_Framework_CostGuardFinding_AlertKey_ImmediateEmailSent]
on [Framework].[CostGuardFinding] ([AlertKey], [ImmediateEmailSent], [Occurred] desc);

GO

create nonclustered index [IX_Framework_CostGuardFinding_Area_Resource_Occurred]
on [Framework].[CostGuardFinding] ([Area], [EnvironmentName], [ResourceName], [Occurred] desc);