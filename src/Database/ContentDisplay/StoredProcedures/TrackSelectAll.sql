create proc [ContentDisplay].[TrackSelectAll] (
     @SessionId uniqueidentifier
) as

declare @contactId uniqueidentifier
declare @portalId uniqueidentifier

select top 1
     @contactId = contact.Id
    ,@portalId = session.PortalId
from [Framework].[Contact-Active] contact
    inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
        and session.Id = @SessionId

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

select
     track.Id
    ,track.Title
    ,track.Description
    ,track.StatusId
    ,track.RequireSequentialCompletion
    ,track.Ordinal
from [Content].[Track-Active] track
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where track.StatusId = @ContentStatusComplete
    and portal.Id = @portalId
    and (track.RequiresAchievementId is null
        or exists (
            select Id
            from [Content].[ContactAchievement-Active]
            where ContactId = @contactId
                and AchievementId = track.RequiresAchievementId
        )
    )
order by track.Ordinal

select
     course.Id
    ,course.Title
    ,course.Description
    ,course.TrackId
    ,course.Ordinal
    ,course.BinderId
from [Content].[Course-Active] course
    inner join [Content].[Track-Active] track on course.TrackId = track.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where track.StatusId = @ContentStatusComplete
    and portal.Id = @portalId
    and (track.RequiresAchievementId is null
        or exists (
            select Id
            from [Content].[ContactAchievement-Active]
            where ContactId = @contactId
                and AchievementId = track.RequiresAchievementId
        )
    )
    and course.StatusId = @ContentStatusComplete
    and (course.RequiresAchievementId is null
        or exists (
            select Id
            from [Content].[ContactAchievement-Active]
            where ContactId = @contactId
                and AchievementId = course.RequiresAchievementId
        )
    )
order by course.Ordinal