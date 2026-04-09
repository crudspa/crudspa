create proc [ContentDesign].[CourseInsert] (
     @SessionId uniqueidentifier
    ,@TrackId uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@Description nvarchar(max)
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@BinderTypeId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Content].[Course-Active] where TrackId = @TrackId)

declare @binderId uniqueidentifier = newid()

insert [Content].[Binder] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TypeId
)
values (
     @binderId
    ,@binderId
    ,@now
    ,@SessionId
    ,@BinderTypeId
)

insert [Content].[Course] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TrackId
    ,Title
    ,StatusId
    ,Description
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,BinderId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@TrackId
    ,@Title
    ,@StatusId
    ,@Description
    ,@RequiresAchievementId
    ,@GeneratesAchievementId
    ,@binderId
    ,@ordinal
)

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

commit transaction