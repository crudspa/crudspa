create table [Education].[Provider] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [OrganizationId] uniqueidentifier not null,
    constraint [PK_Education_Provider] primary key clustered ([Id]),
    constraint [FK_Education_Provider_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
);