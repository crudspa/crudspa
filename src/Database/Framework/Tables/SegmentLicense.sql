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