create view [Content].[PagePane-Active] as

select pagePane.Id as Id
    ,pagePane.PaneId as PaneId
    ,pagePane.PageId as PageId
from [Content].[PagePane] pagePane
where 1=1
    and pagePane.IsDeleted = 0
    and pagePane.VersionOf = pagePane.Id