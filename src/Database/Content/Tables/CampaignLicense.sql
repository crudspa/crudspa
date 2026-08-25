create table [Content].[CampaignLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CampaignId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Content_CampaignLicense] primary key clustered ([Id]),
    constraint [FK_Content_CampaignLicense_Campaign] foreign key ([CampaignId]) references [Content].[Campaign] ([Id]),
    constraint [FK_Content_CampaignLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);

go

create nonclustered index [IX_Content_CampaignLicense_CampaignId_LicenseId]
on [Content].[CampaignLicense] ([CampaignId], [LicenseId])

go

create nonclustered index [IX_Content_CampaignLicense_LicenseId_CampaignId]
on [Content].[CampaignLicense] ([LicenseId], [CampaignId])