create proc [EducationPublisher].[BodyTemplateSelectOrderables] as

set nocount on
select
     bodyTemplate.Id
    ,bodyTemplate.Name as Name
    ,bodyTemplate.Ordinal
from [Education].[BodyTemplate-Active] bodyTemplate
order by bodyTemplate.Ordinal