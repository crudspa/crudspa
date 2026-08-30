create table [Education].[RosterTerm] (
    [Id] uniqueidentifier not null,
    [RosterRunId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SisId] nvarchar(255) null,
    [Name] nvarchar(255) not null,
    [Kind] nvarchar(25) not null,
    [Starts] date null,
    [Ends] date null,
    [Status] nvarchar(25) not null,
    [SourceHash] binary(32) not null,
    constraint [PK_Education_RosterTerm] primary key clustered ([Id]),
    constraint [FK_Education_RosterTerm_RosterRun] foreign key ([RosterRunId]) references [Education].[RosterRun] ([Id]),
    constraint [CK_Education_RosterTerm_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterTerm_Name] check (len(ltrim(rtrim([Name]))) > 0),
    constraint [CK_Education_RosterTerm_Dates] check ([Starts] is null or [Ends] is null or [Ends] >= [Starts]),
    constraint [CK_Education_RosterTerm_Status] check ([Status] in (N'active', N'inactive')),
);

go

create unique nonclustered index [UX_Education_RosterTerm_RunExternal]
    on [Education].[RosterTerm] ([RosterRunId], [ExternalId]);