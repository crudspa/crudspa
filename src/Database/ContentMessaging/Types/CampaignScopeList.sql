create type [ContentMessaging].[CampaignScopeList] as table (
    [ScopeKey] uniqueidentifier not null,
    [ParentScopeKey] uniqueidentifier null,
    [DistrictOrganizationId] uniqueidentifier not null,
    [OrganizationId] uniqueidentifier not null,
    [GradeId] uniqueidentifier null,
    [Name] nvarchar(150) not null,
    [Start] date not null,
    [StartOverridden] bit not null,
    [LessonStart] date not null,
    [LessonStartOverridden] bit not null,
    [AssessmentStart] date not null,
    [AssessmentStartOverridden] bit not null,
    [Ordinal] int not null,
    primary key ([ScopeKey])
);