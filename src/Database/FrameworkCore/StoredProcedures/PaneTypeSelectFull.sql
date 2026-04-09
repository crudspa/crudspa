create proc [FrameworkCore].[PaneTypeSelectFull] (
     @PortalId uniqueidentifier
) as

select
    paneType.Id
    ,paneType.Name
    ,paneType.EditorView
    ,paneType.DisplayView
from [Framework].[PaneType-Active] paneType
    inner join [Framework].[PortalPaneType-Active] portalPaneType on portalPaneType.TypeId = paneType.Id
where portalPaneType.PortalId = @PortalId
order by paneType.Name