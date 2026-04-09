create table [Education].[AchievementViewed] (
    [Id] uniqueidentifier not null,
    [Viewed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [ViewedBy] uniqueidentifier not null,
    [StudentAchievementId] uniqueidentifier null,
    constraint [PK_Education_AchievementViewed] primary key clustered ([Id]),
    constraint [FK_Education_AchievementViewed_StudentAchievement] foreign key ([StudentAchievementId]) references [Education].[StudentAchievement] ([Id]),
);