create view [Education].[ActivityChoice-Active] as

select activityChoice.Id as Id
    ,activityChoice.ActivityId as ActivityId
    ,activityChoice.Text as Text
    ,activityChoice.AudioFileId as AudioFileId
    ,activityChoice.ImageFileId as ImageFileId
    ,activityChoice.IsCorrect as IsCorrect
    ,activityChoice.ColumnOrdinal as ColumnOrdinal
    ,activityChoice.Ordinal as Ordinal
from [Education].[ActivityChoice] activityChoice
where 1=1
    and activityChoice.IsDeleted = 0
    and activityChoice.VersionOf = activityChoice.Id