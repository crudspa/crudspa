create view [Education].[Achievement-Active] as

select achievement.Id as Id
    ,achievement.OwnerId as OwnerId
    ,achievement.Title as Title
    ,achievement.RarityId as RarityId
    ,achievement.TrophyImageId as TrophyImageId
    ,achievement.VisibleToStudents as VisibleToStudents
from [Education].[Achievement] achievement
where 1=1
    and achievement.IsDeleted = 0
    and achievement.VersionOf = achievement.Id