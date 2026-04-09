create proc [EducationStudent].[GameSectionSelectForGame] (
     @GameId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ActivityStatusComplete uniqueidentifier = '2ad6b3d0-3381-4c67-b6f7-5b0c0822dc9d'

select
     gameSection.Id as Id
    ,gameSection.GameId as GameId
    ,gameSection.Title as Title
    ,gameSection.StatusId as StatusId
    ,gameSection.TypeId as TypeId
    ,gameSection.Ordinal as Ordinal
    ,status.Name as StatusName
    ,type.Name as TypeName
    ,(select count(1) from [Education].[GameActivity-Active] gameActivity where gameActivity.SectionId = gameSection.Id) as GameActivityCount
from [Education].[GameSection-Active] gameSection
    inner join [Framework].[ContentStatus-Active] status on gameSection.StatusId = status.Id
    inner join [Education].[GameSectionType-Active] type on gameSection.TypeId = type.Id
where gameSection.GameId = @GameId

select gameActivity.*
from [EducationStudent].[GameActivity-Deep] gameActivity
where gameActivity.GameId = @GameId
    and gameActivity.StatusId = @ActivityStatusComplete
order by gameActivity.Ordinal

select activityChoice.*
from [Education].[ActivityChoice-Deep] activityChoice
    inner join [Education].[Activity-Active] activity on activityChoice.ActivityId = activity.Id
    inner join [Education].[GameActivity-Active] gameActivity on gameActivity.ActivityId = activity.Id
    inner join [Education].[GameSection-Active] gameSection on gameActivity.SectionId = gameSection.Id
where gameSection.GameId = @GameId
    and activity.StatusId = @ActivityStatusComplete