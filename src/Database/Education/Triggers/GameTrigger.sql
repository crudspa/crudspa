create trigger [Education].[GameTrigger] on [Education].[Game]
    for update
as

insert [Education].[Game] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,[Key]
    ,Title
    ,IconName
    ,IconId
    ,StatusId
    ,GradeId
    ,AssessmentLevelId
    ,RequiresAchievementId
    ,GeneratesAchievementId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.[Key]
    ,deleted.Title
    ,deleted.IconName
    ,deleted.IconId
    ,deleted.StatusId
    ,deleted.GradeId
    ,deleted.AssessmentLevelId
    ,deleted.RequiresAchievementId
    ,deleted.GeneratesAchievementId
from deleted