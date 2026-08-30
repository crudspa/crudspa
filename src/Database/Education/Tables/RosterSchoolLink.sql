create table [Education].[RosterSchoolLink] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [RosterSourceId] uniqueidentifier not null,
    [ExternalId] nvarchar(255) not null,
    [SchoolId] uniqueidentifier null,
    [Included] bit not null,
    [SourceHash] binary(32) null,
    constraint [PK_Education_RosterSchoolLink] primary key clustered ([Id]),
    constraint [FK_Education_RosterSchoolLink_RosterSource] foreign key ([RosterSourceId]) references [Education].[RosterSource] ([Id]),
    constraint [FK_Education_RosterSchoolLink_School] foreign key ([SchoolId]) references [Education].[School] ([Id]),
    constraint [CK_Education_RosterSchoolLink_ExternalId] check (len(ltrim(rtrim([ExternalId]))) > 0),
    constraint [CK_Education_RosterSchoolLink_Included] check (([Included] = 0 and [SchoolId] is null) or ([Included] = 1 and [SchoolId] is not null)),
);

go

create nonclustered index [IX_Education_RosterSchoolLink_SourceExternal]
    on [Education].[RosterSchoolLink] ([RosterSourceId], [ExternalId], [IsDeleted], [VersionOf]);