create trigger [Content].[ActivationScopeTrigger] on [Content].[ActivationScope]
    for update
as

insert [Content].[ActivationScope] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ActivationId
    ,ParentId
    ,OrganizationId
    ,Name
    ,Start
    ,StartOverridden
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ActivationId
    ,deleted.ParentId
    ,deleted.OrganizationId
    ,deleted.Name
    ,deleted.Start
    ,deleted.StartOverridden
    ,deleted.Ordinal
from deleted