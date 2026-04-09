create view [Education].[AssignmentBatch-Active] as

select assignmentBatch.Id as Id
    ,assignmentBatch.BookId as BookId
    ,assignmentBatch.GameId as GameId
    ,assignmentBatch.StudentId as StudentId
    ,assignmentBatch.Published as Published
from [Education].[AssignmentBatch] assignmentBatch
where 1=1
    and assignmentBatch.IsDeleted = 0
    and assignmentBatch.VersionOf = assignmentBatch.Id