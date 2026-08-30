create table [Education].[RosterPerson] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SisId] nvarchar(255) null,
    [FirstName] nvarchar(100) not null,
    [LastName] nvarchar(100) not null,
    [Email] nvarchar(255) null,
    [Status] nvarchar(25) not null,
    [AuthIssuer] nvarchar(500) null,
    [AuthSubject] nvarchar(255) null,
    [AssessmentLevel] nvarchar(25) null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterPerson] primary key clustered ([Id]),
    constraint [FK_Education_RosterPerson_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterPerson_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterPerson_FirstName] check (len(ltrim(rtrim([FirstName]))) > 0),
    constraint [CK_Education_RosterPerson_LastName] check (len(ltrim(rtrim([LastName]))) > 0),
    constraint [CK_Education_RosterPerson_Status] check ([Status] in (N'active', N'inactive')),
    constraint [CK_Education_RosterPerson_Auth] check (([AuthIssuer] is null and [AuthSubject] is null) or (len(ltrim(rtrim([AuthIssuer]))) > 0 and len(ltrim(rtrim([AuthSubject]))) > 0)),
    constraint [CK_Education_RosterPerson_AssessmentLevel] check ([AssessmentLevel] is null or [AssessmentLevel] in (N'low', N'mid', N'high')),
);

go

create unique nonclustered index [UX_Education_RosterPerson_RunExternal]
    on [Education].[RosterPerson] ([RosterRunId], [ExternalId]);

go

create nonclustered index [IX_Education_RosterPerson_RunSis]
    on [Education].[RosterPerson] ([RosterRunId], [SisId]);