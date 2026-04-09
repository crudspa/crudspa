create trigger [Education].[ActivityTextEntryTrigger] on [Education].[ActivityTextEntry]
    for update
as

insert [Education].[ActivityTextEntry] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssignmentId
    ,Text
    ,Made
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssignmentId
    ,deleted.Text
    ,deleted.Made
    ,deleted.Ordinal
from deleted