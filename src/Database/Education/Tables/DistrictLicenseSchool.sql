create table [Education].[DistrictLicenseSchool] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [DistrictLicenseId] uniqueidentifier not null,
    [SchoolId] uniqueidentifier not null,
    constraint [PK_Education_DistrictLicenseSchool] primary key clustered ([Id]),
    constraint [FK_Education_DistrictLicenseSchool_DistrictLicense] foreign key ([DistrictLicenseId]) references [Education].[DistrictLicense] ([Id]),
    constraint [FK_Education_DistrictLicenseSchool_School] foreign key ([SchoolId]) references [Education].[School] ([Id]),
);