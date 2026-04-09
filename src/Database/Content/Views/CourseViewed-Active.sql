create view [Content].[CourseViewed-Active] as

select courseViewed.Id as Id
    ,courseViewed.CourseId as CourseId
from [Content].[CourseViewed] courseViewed
where 1=1
    and courseViewed.IsDeleted = 0