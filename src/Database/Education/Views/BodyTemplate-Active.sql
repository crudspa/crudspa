create view [Education].[BodyTemplate-Active] as

select bodyTemplate.Id as Id
    ,bodyTemplate.Name as Name
    ,bodyTemplate.Template as Template
    ,bodyTemplate.Ordinal as Ordinal
from [Education].[BodyTemplate] bodyTemplate
where 1=1
    and bodyTemplate.IsDeleted = 0