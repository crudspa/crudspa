create view [Content].[CourseCompleted-Active] as

select courseCompleted.Id as Id
    ,courseCompleted.ContactId as ContactId
    ,courseCompleted.CourseId as CourseId
    ,courseCompleted.Completed as Completed
from [Content].[CourseCompleted] courseCompleted
where 1=1
    and courseCompleted.IsDeleted = 0