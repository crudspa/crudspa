create proc [EducationPublisher].[SegmentSelectOrderables] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     segment.Id
    ,segment.Title as Name
    ,segment.Ordinal
from [Framework].[Segment-Active] segment
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by segment.Ordinal