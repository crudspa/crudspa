create table [Content].[CampaignSchedule] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ActivationScopeId] uniqueidentifier not null,
    [GradeId] uniqueidentifier null,
    [LessonStart] date not null,
    [LessonStartOverridden] bit default(0) not null,
    [AssessmentStart] date not null,
    [AssessmentStartOverridden] bit default(0) not null,
    constraint [PK_Content_CampaignSchedule] primary key clustered ([Id]),
    constraint [FK_Content_CampaignSchedule_ActivationScope] foreign key ([ActivationScopeId]) references [Content].[ActivationScope] ([Id]),
    constraint [FK_Content_CampaignSchedule_Grade] foreign key ([GradeId]) references [Education].[Grade] ([Id]),
);

go

create nonclustered index [IX_Content_CampaignSchedule_Scope]
    on [Content].[CampaignSchedule] ([ActivationScopeId])
    include ([VersionOf], [IsDeleted], [GradeId], [LessonStart], [AssessmentStart]);