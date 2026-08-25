create table [Framework].[SegmentLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [SegmentId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Framework_SegmentLicense] primary key clustered ([Id]),
    constraint [FK_Framework_SegmentLicense_Segment] foreign key ([SegmentId]) references [Framework].[Segment] ([Id]),
    constraint [FK_Framework_SegmentLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);

go

create nonclustered index [IX_Framework_SegmentLicense_SegmentId_LicenseId]
on [Framework].[SegmentLicense] ([SegmentId], [LicenseId])

go

create nonclustered index [IX_Framework_SegmentLicense_LicenseId_SegmentId]
on [Framework].[SegmentLicense] ([LicenseId], [SegmentId])