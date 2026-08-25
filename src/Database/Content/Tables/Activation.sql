create table [Content].[Activation] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [BatchId] uniqueidentifier not null,
    [CampaignId] uniqueidentifier not null,
    [OrganizationId] uniqueidentifier not null,
    [Start] date null,
    [Activated] datetimeoffset(7) default(sysdatetimeoffset()) null,
    [ActivatedBy] uniqueidentifier null,
    [StatusId] uniqueidentifier default('a9b01002-7c15-4fb8-b25f-9f5629031002') not null,
    constraint [PK_Content_Activation] primary key clustered ([Id]),
    constraint [FK_Content_Activation_Campaign] foreign key ([CampaignId]) references [Content].[Campaign] ([Id]),
    constraint [FK_Content_Activation_Organization] foreign key ([OrganizationId]) references [Framework].[Organization] ([Id]),
    constraint [FK_Content_Activation_ActivatedBy] foreign key ([ActivatedBy]) references [Framework].[User] ([Id]),
    constraint [FK_Content_Activation_Status] foreign key ([StatusId]) references [Content].[ActivationStatus] ([Id]),
);

go

create nonclustered index [IX_Content_Activation_Campaign_Organization]
    on [Content].[Activation] ([CampaignId], [OrganizationId])
    include ([Id], [VersionOf], [IsDeleted], [Start], [Activated], [StatusId]);