create view [Content].[Question-Active] as

select question.Id as Id
    ,question.Text as Text
    ,question.AnswerTypeId as AnswerTypeId
from [Content].[Question] question
where 1=1
    and question.IsDeleted = 0
    and question.VersionOf = question.Id