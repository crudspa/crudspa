create table [Education].[AssessmentLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [AssessmentId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Education_AssessmentLicense] primary key clustered ([Id]),
    constraint [FK_Education_AssessmentLicense_Assessment] foreign key ([AssessmentId]) references [Education].[Assessment] ([Id]),
    constraint [FK_Education_AssessmentLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);