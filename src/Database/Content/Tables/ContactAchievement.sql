create table [Content].[ContactAchievement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [ContactId] uniqueidentifier not null,
    [AchievementId] uniqueidentifier not null,
    [RelatedEntityId] uniqueidentifier not null,
    [Earned] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Content_ContactAchievement] primary key clustered ([Id]),
    constraint [FK_Content_ContactAchievement_Contact] foreign key ([ContactId]) references [Framework].[Contact] ([Id]),
    constraint [FK_Content_ContactAchievement_Achievement] foreign key ([AchievementId]) references [Content].[Achievement] ([Id]),
);