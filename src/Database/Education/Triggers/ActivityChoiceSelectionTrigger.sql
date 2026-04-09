create trigger [Education].[ActivityChoiceSelectionTrigger] on [Education].[ActivityChoiceSelection]
    for update
as

insert [Education].[ActivityChoiceSelection] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssignmentId
    ,ChoiceId
    ,Made
    ,TargetChoiceId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssignmentId
    ,deleted.ChoiceId
    ,deleted.Made
    ,deleted.TargetChoiceId
from deleted