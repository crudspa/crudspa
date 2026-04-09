create proc [EducationDistrict].[TitleSelectOrderables] as

set nocount on

select
     title.Id
    ,title.Name as Name
    ,title.Ordinal
from [Education].[Title-Active] title
order by title.Ordinal