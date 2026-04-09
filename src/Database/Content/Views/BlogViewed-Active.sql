create view [Content].[BlogViewed-Active] as

select blogViewed.Id as Id
    ,blogViewed.BlogId as BlogId
from [Content].[BlogViewed] blogViewed
where 1=1
    and blogViewed.IsDeleted = 0