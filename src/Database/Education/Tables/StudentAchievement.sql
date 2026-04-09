create table [Education].[StudentAchievement] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [StudentId] uniqueidentifier not null,
    [AchievementId] uniqueidentifier not null,
    [RelatedEntityId] uniqueidentifier not null,
    [Earned] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    constraint [PK_Education_StudentAchievement] primary key clustered ([Id]),
    constraint [FK_Education_StudentAchievement_Student] foreign key ([StudentId]) references [Education].[Student] ([Id]),
    constraint [FK_Education_StudentAchievement_Achievement] foreign key ([AchievementId]) references [Education].[Achievement] ([Id]),
);