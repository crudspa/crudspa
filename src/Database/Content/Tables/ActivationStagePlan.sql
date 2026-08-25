create table [Content].[ActivationStagePlan] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ActivationId] uniqueidentifier not null,
    [StageId] uniqueidentifier not null,
    [ScopeLevel] int default(0) not null,
    constraint [PK_Content_ActivationStagePlan] primary key clustered ([Id]),
    constraint [FK_Content_ActivationStagePlan_Activation] foreign key ([ActivationId]) references [Content].[Activation] ([Id]),
    constraint [FK_Content_ActivationStagePlan_Stage] foreign key ([StageId]) references [Content].[Stage] ([Id]),
    constraint [CK_Content_ActivationStagePlan_ScopeLevel] check ([ScopeLevel] between 0 and 2),
);

go

create nonclustered index [IX_Content_ActivationStagePlan_Activation_Stage]
    on [Content].[ActivationStagePlan] ([ActivationId], [StageId])
    include ([VersionOf], [IsDeleted], [ScopeLevel]);