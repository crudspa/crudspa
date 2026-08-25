create proc [EducationPublisher].[TrackSelectOrderables] (
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
     track.Id
    ,track.Title as Name
    ,track.Ordinal
from [Content].[Track-Active] track
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by track.Ordinal