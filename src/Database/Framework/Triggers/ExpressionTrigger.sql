create trigger [Framework].[ExpressionTrigger] on [Framework].[Expression]
    for update
as

insert [Framework].[Expression] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
from deleted