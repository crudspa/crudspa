create table [Education].[RosterRun] (
    [Id] uniqueidentifier not null,
    [RosterSourceId] uniqueidentifier not null,
    [Kind] nvarchar(25) not null,
    [Status] nvarchar(25) not null,
    [Started] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Completed] datetimeoffset(7) null,
    [Checkpoint] nvarchar(500) null,
    [SchoolCount] int default(0) not null,
    [UserCount] int default(0) not null,
    [ClassCount] int default(0) not null,
    [EnrollmentCount] int default(0) not null,
    [AddCount] int default(0) not null,
    [UpdateCount] int default(0) not null,
    [RemoveCount] int default(0) not null,
    [IssueCount] int default(0) not null,
    [TermCount] int default(0) not null,
    [CourseCount] int default(0) not null,
    [RoleCount] int default(0) not null,
    constraint [PK_Education_RosterRun] primary key clustered ([Id]),
    constraint [FK_Education_RosterRun_RosterSource] foreign key ([RosterSourceId]) references [Education].[RosterSource] ([Id]),
    constraint [CK_Education_RosterRun_Kind] check ([Kind] in (N'full', N'delta', N'manual')),
    constraint [CK_Education_RosterRun_Status] check ([Status] in (N'started', N'staged', N'blocked', N'applied', N'failed')),
    constraint [CK_Education_RosterRun_Completed] check ([Completed] is null or [Completed] >= [Started]),
    constraint [CK_Education_RosterRun_Counts] check ([SchoolCount] >= 0 and [UserCount] >= 0 and [ClassCount] >= 0 and [EnrollmentCount] >= 0 and [AddCount] >= 0 and [UpdateCount] >= 0 and [RemoveCount] >= 0 and [IssueCount] >= 0 and [TermCount] >= 0 and [CourseCount] >= 0 and [RoleCount] >= 0),
);

go

create nonclustered index [IX_Education_RosterRun_SourceStarted]
    on [Education].[RosterRun] ([RosterSourceId], [Started] desc);