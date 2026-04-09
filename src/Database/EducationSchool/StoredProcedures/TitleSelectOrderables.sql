create proc [EducationSchool].[TitleSelectOrderables] as

select
    title.Id
    ,title.Name
    ,title.Ordinal
from [Education].[Title-Active] title
order by title.Ordinal