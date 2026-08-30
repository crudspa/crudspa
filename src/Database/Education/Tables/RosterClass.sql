create table [Education].[RosterClass] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SisId] nvarchar(255) null,
    [SchoolExternalId] nvarchar(255) not null,
    [CourseExternalId] nvarchar(255) null,
    [TermExternalId] nvarchar(255) null,
    [Name] nvarchar(255) not null,
    [Grade] nvarchar(25) null,
    [Subject] nvarchar(75) null,
    [Status] nvarchar(25) not null,
    [SmallClassroom] bit null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterClass] primary key clustered ([Id]),
    constraint [FK_Education_RosterClass_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterClass_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterClass_SchoolExternalId] check (len(ltrim(rtrim([SchoolExternalId]))) > 0),
    constraint [CK_Education_RosterClass_Name] check (len(ltrim(rtrim([Name]))) > 0),
    constraint [CK_Education_RosterClass_Status] check ([Status] in (N'active', N'inactive')),
);

go

create unique nonclustered index [UX_Education_RosterClass_RunExternal]
    on [Education].[RosterClass] ([RosterRunId], [ExternalId]);

go

create nonclustered index [IX_Education_RosterClass_RunSchool]
    on [Education].[RosterClass] ([RosterRunId], [SchoolExternalId]);