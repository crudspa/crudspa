create proc [EducationSchool].[ContentCategorySelectOrderables] as

select
    contentCategory.Id
    ,contentCategory.Name
    ,contentCategory.Ordinal
from [Education].[ContentCategory-Active] contentCategory
order by contentCategory.Ordinal