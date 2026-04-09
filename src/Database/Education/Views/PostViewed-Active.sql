create view [Education].[PostViewed-Active] as

select postViewed.Id as Id
    ,postViewed.PostId as PostId
from [Education].[PostViewed] postViewed
where 1=1
    and postViewed.IsDeleted = 0