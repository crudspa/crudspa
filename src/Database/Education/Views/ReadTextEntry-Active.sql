create view [Education].[ReadTextEntry-Active] as

select readTextEntry.Id as Id
    ,readTextEntry.AssignmentId as AssignmentId
    ,readTextEntry.QuestionId as QuestionId
    ,readTextEntry.Text as Text
from [Education].[ReadTextEntry] readTextEntry
where 1=1
    and readTextEntry.IsDeleted = 0