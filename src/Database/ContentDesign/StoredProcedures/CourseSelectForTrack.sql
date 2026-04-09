create proc [ContentDesign].[CourseSelectForTrack] (
     @SessionId uniqueidentifier
    ,@TrackId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     course.Id
    ,course.TrackId
    ,track.Title as TrackTitle
    ,course.Title
    ,course.StatusId
    ,status.Name as StatusName
    ,course.Description
    ,course.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,course.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,course.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
    ,(select count(1) from [Content].[Page-Active] where BinderId = course.BinderId) as PageCount
from [Content].[Course-Active] course
    inner join [Content].[Binder-Active] binder on course.BinderId = binder.Id
    left join [Content].[Achievement-Active] generatesAchievement on course.GeneratesAchievementId = generatesAchievement.Id
    left join [Content].[Achievement-Active] requiresAchievement on course.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on course.StatusId = status.Id
    inner join [Content].[Track-Active] track on course.TrackId = track.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
where course.TrackId = @TrackId
    and organization.Id = @organizationId