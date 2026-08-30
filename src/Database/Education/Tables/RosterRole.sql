create table [Education].[RosterRole] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [PersonExternalId] nvarchar(255) not null,
    [SchoolExternalId] nvarchar(255) null,
    [Role] nvarchar(25) not null,
    [Grade] nvarchar(25) null,
    [Primary] bit not null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterRole] primary key clustered ([Id]),
    constraint [FK_Education_RosterRole_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterRole_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterRole_PersonExternalId] check (len(ltrim(rtrim([PersonExternalId]))) > 0),
    constraint [CK_Education_RosterRole_Role] check ([Role] in (N'student', N'teacher', N'staff', N'principal', N'literacy-facilitator', N'teacher-leader', N'district-admin', N'contact')),
);

go

create unique nonclustered index [UX_Education_RosterRole_RunExternal]
    on [Education].[RosterRole] ([RosterRunId], [ExternalId]);

go

create nonclustered index [IX_Education_RosterRole_RunPerson]
    on [Education].[RosterRole] ([RosterRunId], [PersonExternalId]);