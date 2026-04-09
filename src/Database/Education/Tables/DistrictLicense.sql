create table [Education].[DistrictLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [DistrictId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    [AllSchools] bit default(1) not null,
    constraint [PK_Education_DistrictLicense] primary key clustered ([Id]),
    constraint [FK_Education_DistrictLicense_District] foreign key ([DistrictId]) references [Education].[District] ([Id]),
    constraint [FK_Education_DistrictLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);