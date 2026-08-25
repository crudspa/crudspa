create trigger [Content].[ActivationStagePlanTrigger] on [Content].[ActivationStagePlan]
    for update
as

insert [Content].[ActivationStagePlan] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ActivationId
    ,StageId
    ,ScopeLevel
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ActivationId
    ,deleted.StageId
    ,deleted.ScopeLevel
from deleted