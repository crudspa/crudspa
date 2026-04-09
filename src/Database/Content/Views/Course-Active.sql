create view [Content].[Course-Active] as

select course.Id as Id
    ,course.TrackId as TrackId
    ,course.BinderId as BinderId
    ,course.Title as Title
    ,course.Description as Description
    ,course.StatusId as StatusId
    ,course.RequiresAchievementId as RequiresAchievementId
    ,course.GeneratesAchievementId as GeneratesAchievementId
    ,course.Ordinal as Ordinal
from [Content].[Course] course
where 1=1
    and course.IsDeleted = 0
    and course.VersionOf = course.Id