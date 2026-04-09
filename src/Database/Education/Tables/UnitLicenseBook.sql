create table [Education].[UnitLicenseBook] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UnitLicenseId] uniqueidentifier not null,
    [BookId] uniqueidentifier not null,
    constraint [PK_Education_UnitLicenseBook] primary key clustered ([Id]),
    constraint [FK_Education_UnitLicenseBook_UnitLicense] foreign key ([UnitLicenseId]) references [Education].[UnitLicense] ([Id]),
    constraint [FK_Education_UnitLicenseBook_Book] foreign key ([BookId]) references [Education].[Book] ([Id]),
);