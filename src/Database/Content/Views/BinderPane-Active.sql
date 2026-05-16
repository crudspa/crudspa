create view [Content].[BinderPane-Active] as

select binderPane.Id as Id
    ,binderPane.PaneId as PaneId
    ,binderPane.BinderId as BinderId
from [Content].[BinderPane] binderPane
where 1=1
    and binderPane.IsDeleted = 0
    and binderPane.VersionOf = binderPane.Id