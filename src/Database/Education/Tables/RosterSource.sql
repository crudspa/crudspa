create table [Education].[RosterSource] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    [Provider] nvarchar(75) not null,
    [Tenant] nvarchar(255) null,
    [ClientId] nvarchar(255) null,
    [ClientSecret] nvarchar(500) null,
    [TokenUrl] nvarchar(500) null,
    [BaseUrl] nvarchar(500) null,
    [Mode] nvarchar(25) not null,
    [ScheduleId] uniqueidentifier null,
    [Checkpoint] nvarchar(500) null,
    [LastSucceeded] datetimeoffset(7) null,
    [Recurring] bit default(0) not null,
    [ScheduleHour] int null,
    [ScheduleMinute] int null,
    [ScheduleTimeZoneId] nvarchar(32) null,
    constraint [PK_Education_RosterSource] primary key clustered ([Id]),
    constraint [FK_Education_RosterSource_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Education_RosterSource_Schedule] foreign key ([ScheduleId]) references [Framework].[JobSchedule] ([Id]),
    constraint [CK_Education_RosterSource_Provider] check (len(ltrim(rtrim([Provider]))) > 0),
    constraint [CK_Education_RosterSource_Tenant] check ([Tenant] is null or len(ltrim(rtrim([Tenant]))) > 0),
    constraint [CK_Education_RosterSource_ClientId] check ([ClientId] is null or len(ltrim(rtrim([ClientId]))) > 0),
    constraint [CK_Education_RosterSource_ClientSecret] check ([ClientSecret] is null or len(ltrim(rtrim([ClientSecret]))) > 0),
    constraint [CK_Education_RosterSource_TokenUrl] check ([TokenUrl] is null or len(ltrim(rtrim([TokenUrl]))) > 0),
    constraint [CK_Education_RosterSource_BaseUrl] check ([BaseUrl] is null or len(ltrim(rtrim([BaseUrl]))) > 0),
    constraint [CK_Education_RosterSource_Mode] check ([Mode] in (N'disabled', N'shadow', N'authoritative')),
    constraint [CK_Education_RosterSource_Recurring] check ([Recurring] = 0 or ([Mode] <> N'disabled' and [ScheduleHour] between 0 and 23 and [ScheduleMinute] between 0 and 59 and len(ltrim(rtrim([ScheduleTimeZoneId]))) > 0)),
);

go

create nonclustered index [IX_Education_RosterSource_OrganizationProvider]
    on [Education].[RosterSource] ([OrganizationId], [Provider], [IsDeleted], [VersionOf]);