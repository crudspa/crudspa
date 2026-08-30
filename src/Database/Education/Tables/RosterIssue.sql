create table [Education].[RosterIssue] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [Kind] nvarchar(25) not null,
    [ExternalId] nvarchar(255) null,
    [Severity] nvarchar(25) not null,
    [Code] nvarchar(75) not null,
    [Detail] nvarchar(500) null,
    constraint [PK_Education_RosterIssue] primary key clustered ([Id]),
    constraint [FK_Education_RosterIssue_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterIssue_Kind] check ([Kind] in (N'source', N'school', N'term', N'course', N'class', N'person', N'role', N'enrollment')),
    constraint [CK_Education_RosterIssue_Severity] check ([Severity] in (N'warning', N'blocking')),
    constraint [CK_Education_RosterIssue_Code] check (len(ltrim(rtrim([Code]))) > 0),
);

go

create nonclustered index [IX_Education_RosterIssue_RunSeverity]
    on [Education].[RosterIssue] ([RosterRunId], [Severity], [Kind]);