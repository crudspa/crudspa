create view [Framework].[PaneType-Active] as

select paneType.Id as Id
    ,paneType.Name as Name
    ,paneType.EditorView as EditorView
    ,paneType.DisplayView as DisplayView
from [Framework].[PaneType] paneType
where 1=1
    and paneType.IsDeleted = 0