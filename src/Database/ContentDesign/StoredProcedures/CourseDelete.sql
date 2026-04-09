create proc [ContentDesign].[CourseDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @trackId uniqueidentifier = (select top 1 TrackId from [Content].[Course] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[Course-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[Course-Active] course
        inner join [Content].[Track-Active] track on course.TrackId = track.Id
        inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where course.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update course
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Course] course
where course.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[Course] baseTable
    inner join [Content].[Course-Active] course on course.Id = baseTable.Id
where course.TrackId = @trackId
    and course.Ordinal > @oldOrdinal

commit transaction