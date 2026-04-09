create view [Education].[GameSection-Active] as

select gameSection.Id as Id
    ,gameSection.GameId as GameId
    ,gameSection.Title as Title
    ,gameSection.StatusId as StatusId
    ,gameSection.TypeId as TypeId
    ,gameSection.Ordinal as Ordinal
from [Education].[GameSection] gameSection
where 1=1
    and gameSection.IsDeleted = 0
    and gameSection.VersionOf = gameSection.Id