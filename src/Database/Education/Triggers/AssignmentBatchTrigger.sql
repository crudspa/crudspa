create trigger [Education].[AssignmentBatchTrigger] on [Education].[AssignmentBatch]
    for update
as

insert [Education].[AssignmentBatch] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,GameId
    ,StudentId
    ,Published
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.GameId
    ,deleted.StudentId
    ,deleted.Published
from deleted