create table [Education].[RosterLink] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [RosterSourceId] uniqueidentifier not null,
    [Kind] nvarchar(25) not null,
    [ExternalId] nvarchar(255) not null,
    [LocalId] uniqueidentifier not null,
    [SourceHash] binary(32) null,
    constraint [PK_Education_RosterLink] primary key clustered ([Id]),
    constraint [FK_Education_RosterLink_RosterSource] foreign key ([RosterSourceId]) references [Education].[RosterSource] ([Id]),
    constraint [CK_Education_RosterLink_Kind] check ([Kind] in (N'school', N'term', N'course', N'class', N'person', N'role', N'enrollment')),
    constraint [CK_Education_RosterLink_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
);

go

create nonclustered index [IX_Education_RosterLink_SourceExternal]
    on [Education].[RosterLink] ([RosterSourceId], [Kind], [ExternalId], [IsDeleted], [VersionOf]);