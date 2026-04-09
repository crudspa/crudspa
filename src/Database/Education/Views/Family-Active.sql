create view [Education].[Family-Active] as

select family.Id as Id
    ,family.OrganizationId as OrganizationId
    ,family.SchoolId as SchoolId
    ,family.ImportNum as ImportNum
    ,family.SurveyComplete as SurveyComplete
from [Education].[Family] family
where 1=1
    and family.IsDeleted = 0
    and family.VersionOf = family.Id