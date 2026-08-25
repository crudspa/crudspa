create table [Content].[TrackLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [TrackId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Content_TrackLicense] primary key clustered ([Id]),
    constraint [FK_Content_TrackLicense_Track] foreign key ([TrackId]) references [Content].[Track] ([Id]),
    constraint [FK_Content_TrackLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);

go

create nonclustered index [IX_Content_TrackLicense_TrackId_LicenseId]
on [Content].[TrackLicense] ([TrackId], [LicenseId])

go

create nonclustered index [IX_Content_TrackLicense_LicenseId_TrackId]
on [Content].[TrackLicense] ([LicenseId], [TrackId])