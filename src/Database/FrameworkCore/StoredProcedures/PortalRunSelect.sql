create proc [FrameworkCore].[PortalRunSelect] (
     @Id uniqueidentifier
) as

set nocount on

select
     portal.Id
    ,portal.[Key]
    ,portal.Title
    ,portal.SessionsPersist
    ,portal.AllowSignIn
    ,portal.RequireSignIn
    ,navigationType.DisplayView as NavigationTypeDisplayView
from [Framework].[Portal-Active] portal
    inner join [Framework].[NavigationType-Active] navigationType on portal.NavigationTypeId = navigationType.Id
where portal.Id = @Id