create table [Education].[Publisher] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    [ProviderId] uniqueidentifier not null,
    constraint [PK_Education_Publisher] primary key clustered ([Id]),
    constraint [FK_Education_Publisher_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Education_Publisher_Provider] foreign key ([ProviderId]) references [Education].[Provider] ([Id]),
);