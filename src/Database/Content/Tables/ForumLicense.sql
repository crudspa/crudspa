create table [Content].[ForumLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ForumId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Content_ForumLicense] primary key clustered ([Id]),
    constraint [FK_Content_ForumLicense_Forum] foreign key ([ForumId]) references [Content].[Forum] ([Id]),
    constraint [FK_Content_ForumLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);