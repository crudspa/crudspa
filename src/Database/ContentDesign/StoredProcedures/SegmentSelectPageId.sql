create proc [ContentDesign].[SegmentSelectPageId] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@PageId uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @PageId = (
    select
        case when count(*) = 1 then min(try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId'))) end
    from [Framework].[Pane-Active] pane
        inner join [Framework].[Segment-Active] segment on pane.SegmentId = segment.Id
        inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where segment.Id = @Id
        and organization.Id = @organizationId
        and try_convert(uniqueidentifier, json_value(pane.ConfigJson, '$.PageId')) is not null
)