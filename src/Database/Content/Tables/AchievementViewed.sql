create table [Content].[AchievementViewed] (
    [Id] uniqueidentifier not null,
    [Viewed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [ViewedBy] uniqueidentifier not null,
    [ContactAchievementId] uniqueidentifier null,
    constraint [PK_Content_AchievementViewed] primary key clustered ([Id]),
    constraint [FK_Content_AchievementViewed_ContactAchievement] foreign key ([ContactAchievementId]) references [Content].[ContactAchievement] ([Id]),
);