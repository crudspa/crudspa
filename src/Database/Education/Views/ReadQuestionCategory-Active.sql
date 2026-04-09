create view [Education].[ReadQuestionCategory-Active] as

select readQuestionCategory.Id as Id
    ,readQuestionCategory.Name as Name
    ,readQuestionCategory.Ordinal as Ordinal
from [Education].[ReadQuestionCategory] readQuestionCategory
where 1=1
    and readQuestionCategory.IsDeleted = 0