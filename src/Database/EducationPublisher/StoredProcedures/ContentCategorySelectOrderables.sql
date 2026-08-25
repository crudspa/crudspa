create proc [EducationPublisher].[ContentCategorySelectOrderables] as

set nocount on

select
     contentCategory.Id
    ,contentCategory.Name as Name
    ,contentCategory.Ordinal
from [Education].[ContentCategory-Active] contentCategory
where contentCategory.Name <> N'Other'
order by contentCategory.Ordinal