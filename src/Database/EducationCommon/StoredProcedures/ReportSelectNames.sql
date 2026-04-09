create proc [EducationCommon].[ReportSelectNames] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
)
as

declare @ownerId uniqueidentifier = (
    select top 1 portal.OwnerId
    from [Framework].[Session-Active] session
        inner join [Framework].[Portal-Active] portal on session.PortalId = portal.Id
    where session.Id = @SessionId
)

set nocount on

select
     report.Id
    ,report.Name
from [Education].[Report-Active] report
    inner join [Framework].[Portal-Active] portal on report.PortalId = portal.Id
where report.PortalId = @PortalId
    and portal.OwnerId = @ownerId
order by report.Ordinal