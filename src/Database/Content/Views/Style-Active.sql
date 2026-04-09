create view [Content].[Style-Active] as

select style.Id as Id
    ,style.ContentPortalId as ContentPortalId
    ,style.RuleId as RuleId
    ,style.ConfigJson as ConfigJson
from [Content].[Style] style
where 1=1
    and style.IsDeleted = 0
    and style.VersionOf = style.Id