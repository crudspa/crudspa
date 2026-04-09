create view [EducationStudent].[GameActivity-Deep] as

select distinct
    activity.*
    ,gameActivity.Id as GameActivityId
    ,gameActivity.SectionId as SectionId
    ,gameActivity.ActivityId as GameActivityActivityId
    ,gameActivity.ThemeWord as ThemeWord
    ,gameActivity.Rigorous as Rigorous
    ,gameActivity.GroupId as GroupId
    ,gameActivity.Ordinal as Ordinal
    ,gameSection.Title as GameSectionTitle
    ,game.Id as GameId
    ,game.[Key] as GameKey
    ,game.Title as GameTitle
    ,game.BookId as GameBookId
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[GameSection-Active] gameSection on gameActivity.SectionId = gameSection.Id
    inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
    inner join [Education].[Activity-Deep] activity on gameActivity.ActivityId = activity.Id