create view [Education].[ChapterCompleted-Active] as

select chapterCompleted.Id as Id
    ,chapterCompleted.StudentId as StudentId
    ,chapterCompleted.ChapterId as ChapterId
    ,chapterCompleted.DeviceTimestamp as DeviceTimestamp
from [Education].[ChapterCompleted] chapterCompleted
where 1=1
    and chapterCompleted.IsDeleted = 0