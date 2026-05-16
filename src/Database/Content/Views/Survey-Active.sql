create view [Content].[Survey-Active] as

select survey.Id as Id
    ,survey.PortalId as PortalId
    ,survey.Title as Title
    ,survey.Description as Description
    ,survey.StatusId as StatusId
    ,survey.AssignmentKind as AssignmentKind
from [Content].[Survey] survey
where 1=1
    and survey.IsDeleted = 0
    and survey.VersionOf = survey.Id