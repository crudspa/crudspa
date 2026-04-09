create view [Education].[LessonCompleted-Active] as

select lessonCompleted.Id as Id
    ,lessonCompleted.StudentId as StudentId
    ,lessonCompleted.LessonId as LessonId
    ,lessonCompleted.Completed as Completed
from [Education].[LessonCompleted] lessonCompleted
where 1=1
    and lessonCompleted.IsDeleted = 0