create trigger [Content].[StyleTrigger] on [Content].[Style]
    for update
as

insert [Content].[Style] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContentPortalId
    ,RuleId
    ,ConfigJson
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContentPortalId
    ,deleted.RuleId
    ,deleted.ConfigJson
from deleted