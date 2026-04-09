create table [Education].[UnitLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UnitId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    [AllBooks] bit default(1) not null,
    [AllLessons] bit default(1) not null,
    constraint [PK_Education_UnitLicense] primary key clustered ([Id]),
    constraint [FK_Education_UnitLicense_Unit] foreign key ([UnitId]) references [Education].[Unit] ([Id]),
    constraint [FK_Education_UnitLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);