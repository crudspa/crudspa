create table [Content].[SurveyLicense] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [SurveyId] uniqueidentifier not null,
    [LicenseId] uniqueidentifier not null,
    constraint [PK_Content_SurveyLicense] primary key clustered ([Id]),
    constraint [FK_Content_SurveyLicense_Survey] foreign key ([SurveyId]) references [Content].[Survey] ([Id]),
    constraint [FK_Content_SurveyLicense_License] foreign key ([LicenseId]) references [Framework].[License] ([Id]),
);

go

create nonclustered index [IX_Content_SurveyLicense_SurveyId_LicenseId]
on [Content].[SurveyLicense] ([SurveyId], [LicenseId])

go

create nonclustered index [IX_Content_SurveyLicense_LicenseId_SurveyId]
on [Content].[SurveyLicense] ([LicenseId], [SurveyId])