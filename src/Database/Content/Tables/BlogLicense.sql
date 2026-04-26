create table [Content].[BlogLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BlogId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Content_BlogLicense] primary key clustered ([Id]),
    constraint [FK_Content_BlogLicense_Blog] foreign key ([BlogId]) references [Content].[Blog] ([Id]),
    constraint [FK_Content_BlogLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);