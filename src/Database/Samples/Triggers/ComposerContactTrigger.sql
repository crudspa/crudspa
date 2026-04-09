create trigger [Samples].[ComposerContactTrigger] on [Samples].[ComposerContact]
    for update
as

insert [Samples].[ComposerContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,UserId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.UserId
from deleted