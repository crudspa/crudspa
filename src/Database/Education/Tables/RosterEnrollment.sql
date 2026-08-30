create table [Education].[RosterEnrollment] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [PersonExternalId] nvarchar(255) not null,
    [ClassExternalId] nvarchar(255) not null,
    [SchoolExternalId] nvarchar(255) null,
    [Role] nvarchar(25) not null,
    [Primary] bit not null,
    [Status] nvarchar(25) not null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterEnrollment] primary key clustered ([Id]),
    constraint [FK_Education_RosterEnrollment_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterEnrollment_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterEnrollment_PersonExternalId] check (len(ltrim(rtrim([PersonExternalId]))) > 0),
    constraint [CK_Education_RosterEnrollment_ClassExternalId] check (len(ltrim(rtrim([ClassExternalId]))) > 0),
    constraint [CK_Education_RosterEnrollment_Role] check ([Role] in (N'student', N'teacher')),
    constraint [CK_Education_RosterEnrollment_Status] check ([Status] in (N'active', N'inactive')),
);

go

create unique nonclustered index [UX_Education_RosterEnrollment_RunExternal]
    on [Education].[RosterEnrollment] ([RosterRunId], [ExternalId]);

go

create nonclustered index [IX_Education_RosterEnrollment_RunPerson]
    on [Education].[RosterEnrollment] ([RosterRunId], [PersonExternalId]);

go

create nonclustered index [IX_Education_RosterEnrollment_RunClass]
    on [Education].[RosterEnrollment] ([RosterRunId], [ClassExternalId]);