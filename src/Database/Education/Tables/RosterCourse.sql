create table [Education].[RosterCourse] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SisId] nvarchar(255) null,
    [Name] nvarchar(255) not null,
    [Number] nvarchar(75) null,
    [Status] nvarchar(25) not null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterCourse] primary key clustered ([Id]),
    constraint [FK_Education_RosterCourse_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterCourse_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterCourse_Name] check (len(ltrim(rtrim([Name]))) > 0),
    constraint [CK_Education_RosterCourse_Status] check ([Status] in (N'active', N'inactive')),
);

go

create unique nonclustered index [UX_Education_RosterCourse_RunExternal]
    on [Education].[RosterCourse] ([RosterRunId], [ExternalId]);