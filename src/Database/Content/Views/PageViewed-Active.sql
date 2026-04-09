create view [Content].[PageViewed-Active] as

select pageViewed.Id as Id
    ,pageViewed.PageId as PageId
from [Content].[PageViewed] pageViewed
where 1=1
    and pageViewed.IsDeleted = 0