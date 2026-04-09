create view [Education].[LessonViewed-Active] as

select lessonViewed.Id as Id
    ,lessonViewed.LessonId as LessonId
from [Education].[LessonViewed] lessonViewed
where 1=1
    and lessonViewed.IsDeleted = 0