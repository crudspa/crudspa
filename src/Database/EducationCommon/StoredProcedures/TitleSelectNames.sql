create proc [EducationCommon].[TitleSelectNames] as

select
    title.Id
    ,title.[Name] as Name
    ,title.Ordinal as Ordinal
from [Education].[Title-Active] as title
order by title.Ordinal