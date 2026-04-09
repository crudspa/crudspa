create proc [ContentDesign].[CourseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@Description nvarchar(max)
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@BinderTypeId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

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

declare @binderId uniqueidentifier = (select top 1 BinderId from [Content].[Course] where Id = @Id)

update [Content].[Binder]
set
     Id = @binderId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @BinderTypeId
where Id = @binderId

update course
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,Description = @Description
    ,RequiresAchievementId = @RequiresAchievementId
    ,GeneratesAchievementId = @GeneratesAchievementId
from [Content].[Course] course
where course.Id = @Id

commit transaction