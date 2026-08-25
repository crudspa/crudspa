create table [Content].[ActivationStage] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [PlanId] uniqueidentifier not null,
    [ActivationScopeId] uniqueidentifier not null,
    [Send] datetimeoffset(7) not null,
    [Overridden] bit default(0) not null,
    constraint [PK_Content_ActivationStage] primary key clustered ([Id]),
    constraint [FK_Content_ActivationStage_Plan] foreign key ([PlanId]) references [Content].[ActivationStagePlan] ([Id]),
    constraint [FK_Content_ActivationStage_ActivationScope] foreign key ([ActivationScopeId]) references [Content].[ActivationScope] ([Id]),
);

go

create nonclustered index [IX_Content_ActivationStage_Plan_Scope]
    on [Content].[ActivationStage] ([PlanId], [ActivationScopeId])
    include ([VersionOf], [IsDeleted], [Send]);