create proc [ContentDesign].[TrackSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
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
    ,track.PortalId
    ,portal.[Key] as PortalKey
    ,track.Title
    ,track.StatusId
    ,status.Name as StatusName
    ,track.Description
    ,track.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,track.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,track.RequireSequentialCompletion
    ,track.Ordinal
    ,(select count(1) from [Content].[Course-Active] where TrackId = track.Id) as CourseCount
from [Content].[Track-Active] track
    left join [Content].[Achievement-Active] generatesAchievement on track.GeneratesAchievementId = generatesAchievement.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    left join [Content].[Achievement-Active] requiresAchievement on track.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on track.StatusId = status.Id
where track.PortalId = @PortalId
    and organization.Id = @organizationId