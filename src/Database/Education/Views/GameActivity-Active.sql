create view [Education].[GameActivity-Active] as

select gameActivity.Id as Id
    ,gameActivity.SectionId as SectionId
    ,gameActivity.ActivityId as ActivityId
    ,gameActivity.ThemeWord as ThemeWord
    ,gameActivity.Rigorous as Rigorous
    ,gameActivity.Multisyllabic as Multisyllabic
    ,gameActivity.GroupId as GroupId
    ,gameActivity.TypeId as TypeId
    ,gameActivity.Ordinal as Ordinal
from [Education].[GameActivity] gameActivity
where 1=1
    and gameActivity.IsDeleted = 0
    and gameActivity.VersionOf = gameActivity.Id