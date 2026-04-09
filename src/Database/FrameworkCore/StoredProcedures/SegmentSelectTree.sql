create proc [FrameworkCore].[SegmentSelectTree] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
     portal.Id
    ,portal.Title
from [Framework].[Portal-Active] portal
    inner join [Framework].[Organization-Active] owner on portal.OwnerId = owner.Id
where portal.OwnerId = @organizationId

;with SegmentCte as (

    select
         segment.Id
        ,segment.[Key]
        ,segment.Title
        ,segment.PortalId
        ,segment.ParentId
        ,segment.IconId
        ,segment.Ordinal
    from [Framework].[Segment-Active] segment
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    where segment.PortalId is not null
        and segment.ParentId is null
        and portal.OwnerId = @organizationId

    union all

    select
         segment.Id
        ,segment.[Key]
        ,segment.Title
        ,segment.PortalId
        ,segment.ParentId
        ,segment.IconId
        ,segment.Ordinal
    from [Framework].[Segment-Active] segment
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join SegmentCte cte on segment.ParentId = cte.Id
    where portal.OwnerId = @organizationId
)

select distinct
     cte.Id
    ,cte.[Key]
    ,cte.Title
    ,cte.PortalId
    ,cte.ParentId
    ,cte.Ordinal
    ,icon.CssClass
from SegmentCte cte
    left join [Framework].[Icon-Active] icon on cte.IconId = icon.Id