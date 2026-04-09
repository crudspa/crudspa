create proc [EducationPublisher].[ReadQuestionCategorySelectOrderables] as

set nocount on
select
     readQuestionCategory.Id
    ,readQuestionCategory.Name as Name
    ,readQuestionCategory.Ordinal
from [Education].[ReadQuestionCategory-Active] readQuestionCategory
order by readQuestionCategory.Ordinal