create table [Content].[Stage] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [CampaignId] uniqueidentifier not null,
    [PopulationId] uniqueidentifier null,
    [Name] nvarchar(75) not null,
    [Offset] int not null,
    [Anchor] int default(1) not null,
    [MessageTypeId] uniqueidentifier null,
    [EmailTemplateId] uniqueidentifier null,
    [SmsTemplateId] uniqueidentifier null,
    [WeekendAdjustment] int default(0) not null,
    [SendTime] time not null,
    [Ordinal] int not null,
    constraint [PK_Content_Stage] primary key clustered ([Id]),
    constraint [FK_Content_Stage_Campaign] foreign key ([CampaignId]) references [Content].[Campaign] ([Id]),
    constraint [FK_Content_Stage_Population] foreign key ([PopulationId]) references [Content].[Population] ([Id]),
    constraint [FK_Content_Stage_MessageType] foreign key ([MessageTypeId]) references [Content].[MessageType] ([Id]),
    constraint [FK_Content_Stage_EmailTemplate] foreign key ([EmailTemplateId]) references [Content].[EmailTemplate] ([Id]),
    constraint [FK_Content_Stage_SmsTemplate] foreign key ([SmsTemplateId]) references [Content].[SmsTemplate] ([Id]),
    constraint [CK_Content_Stage_Anchor] check ([Anchor] between 0 and 2),
    constraint [CK_Content_Stage_WeekendAdjustment] check ([WeekendAdjustment] between 0 and 2),
);

go

create nonclustered index [IX_Content_Stage_Campaign_Ordinal]
    on [Content].[Stage] ([CampaignId], [Ordinal])
    include ([Id], [VersionOf], [IsDeleted]);