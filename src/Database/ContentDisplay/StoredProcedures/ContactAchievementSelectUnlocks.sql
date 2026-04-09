create proc [ContentDisplay].[ContactAchievementSelectUnlocks] (
     @ContactId uniqueidentifier
    ,@AchievementId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

select
    course.Id
    ,course.Title
    ,course.Description
    ,course.TrackId
    ,track.Title as TrackTitle
    ,track.Description as TrackDescription
from [Content].[Course-Active] course
    inner join [Content].[Track-Active] track on course.TrackId = track.Id
where course.StatusId = @ContentStatusComplete
    and track.StatusId = @ContentStatusComplete
    and course.RequiresAchievementId = @AchievementId

select
    track.Id
    ,track.Title
    ,track.Description
from [Content].[Track-Active] track
where track.StatusId = @ContentStatusComplete
    and track.RequiresAchievementId = @AchievementId