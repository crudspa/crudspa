create trigger [Content].[SurveyTrigger] on [Content].[Survey]
    for update
as

insert [Content].[Survey] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Title
    ,Description
    ,StatusId
    ,AssignmentKind
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.Title
    ,deleted.Description
    ,deleted.StatusId
    ,deleted.AssignmentKind
from deleted