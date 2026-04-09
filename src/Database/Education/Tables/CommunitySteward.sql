create table [Education].[CommunitySteward] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CommunityId] uniqueidentifier not null,
    [DistrictContactId] uniqueidentifier not null,
    constraint [PK_Education_CommunitySteward] primary key clustered ([Id]),
    constraint [FK_Education_CommunitySteward_Community] foreign key ([CommunityId]) references [Education].[Community] ([Id]),
    constraint [FK_Education_CommunitySteward_DistrictContact] foreign key ([DistrictContactId]) references [Education].[DistrictContact] ([Id]),
);