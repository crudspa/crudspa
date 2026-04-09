create trigger [Education].[BookGradeTrigger] on [Education].[BookGrade]
    for update
as

insert [Education].[BookGrade] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BookId
    ,GradeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BookId
    ,deleted.GradeId
from deleted