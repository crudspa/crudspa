create proc [ContentDesign].[CourseSelectNames] (
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
     course.Id
    ,track.Title + ' | ' + course.Title as Name
from [Content].[Course-Active] course
    inner join [Content].[Track-Active] track on course.TrackId = track.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where track.PortalId = @PortalId
    and organization.Id = @organizationId
order by track.Ordinal, course.Ordinal