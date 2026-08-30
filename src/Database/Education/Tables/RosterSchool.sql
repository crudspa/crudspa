create table [Education].[RosterSchool] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SisId] nvarchar(255) null,
    [Name] nvarchar(255) not null,
    [Kind] nvarchar(25) not null,
    [Status] nvarchar(25) not null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterSchool] primary key clustered ([Id]),
    constraint [FK_Education_RosterSchool_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterSchool_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterSchool_Name] check (len(ltrim(rtrim([Name]))) > 0),
    constraint [CK_Education_RosterSchool_Kind] check ([Kind] in (N'school', N'district-office')),
    constraint [CK_Education_RosterSchool_Status] check ([Status] in (N'active', N'inactive')),
);

go

create unique nonclustered index [UX_Education_RosterSchool_RunExternal]
    on [Education].[RosterSchool] ([RosterRunId], [ExternalId]);