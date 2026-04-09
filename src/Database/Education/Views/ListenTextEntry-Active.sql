create view [Education].[ListenTextEntry-Active] as

select listenTextEntry.Id as Id
    ,listenTextEntry.AssignmentId as AssignmentId
    ,listenTextEntry.QuestionId as QuestionId
    ,listenTextEntry.Text as Text
from [Education].[ListenTextEntry] listenTextEntry
where 1=1
    and listenTextEntry.IsDeleted = 0