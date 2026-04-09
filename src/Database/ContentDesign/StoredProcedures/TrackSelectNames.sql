create proc [ContentDesign].[TrackSelectNames] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
)
as

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
from [Content].[Track-Active] track
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where track.PortalId = @PortalId
    and organization.Id = @organizationId
order by track.Ordinal