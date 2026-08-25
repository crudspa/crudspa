create trigger [Content].[ActivationStageTrigger] on [Content].[ActivationStage]
    for update
as

insert [Content].[ActivationStage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PlanId
    ,ActivationScopeId
    ,Send
    ,Overridden
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PlanId
    ,deleted.ActivationScopeId
    ,deleted.Send
    ,deleted.Overridden
from deleted