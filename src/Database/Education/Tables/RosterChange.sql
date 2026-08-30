create table [Education].[RosterChange] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [Kind] nvarchar(25) not null,
    [ExternalId] nvarchar(255) not null,
    [LocalId] uniqueidentifier null,
    [Action] nvarchar(25) not null,
    [Severity] nvarchar(25) not null,
    [Code] nvarchar(75) not null,
    constraint [PK_Education_RosterChange] primary key clustered ([Id]),
    constraint [FK_Education_RosterChange_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterChange_Kind] check ([Kind] in (N'school', N'term', N'course', N'class', N'person', N'role', N'enrollment')),
    constraint [CK_Education_RosterChange_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterChange_Action] check ([Action] in (N'add', N'update', N'remove', N'unchanged', N'conflict', N'excluded')),
    constraint [CK_Education_RosterChange_Severity] check ([Severity] in (N'info', N'warning', N'blocking')),
    constraint [CK_Education_RosterChange_Code] check (len(ltrim(rtrim([Code]))) > 0),
);

go

create unique nonclustered index [UX_Education_RosterChange_RunExternal]
    on [Education].[RosterChange] ([RosterRunId], [Kind], [ExternalId]);