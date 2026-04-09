create view [Education].[ReadQuestionType-Active] as

select readQuestionType.Id as Id
    ,readQuestionType.Name as Name
    ,readQuestionType.Ordinal as Ordinal
from [Education].[ReadQuestionType] readQuestionType
where 1=1
    and readQuestionType.IsDeleted = 0