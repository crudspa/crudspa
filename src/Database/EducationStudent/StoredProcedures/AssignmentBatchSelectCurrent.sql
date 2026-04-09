create proc [EducationStudent].[AssignmentBatchSelectCurrent] (
     @StudentId uniqueidentifier
    ,@GameId uniqueidentifier
) as

select top 1
     gameRun.Id
    ,gameRun.Published
from [Education].[AssignmentBatch-Active] gameRun
where  gameRun.StudentId = @StudentId
    and gameRun.GameId = @GameId
    and gameRun.Id not in (select GameRunId from [Education].[GameCompleted-Active] where GameRunId is not null)
order by gameRun.Published desc