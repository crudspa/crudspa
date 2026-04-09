create proc [EducationPublisher].[ReadQuestionTypeSelectOrderables] as

set nocount on
select
     readQuestionType.Id
    ,readQuestionType.Name as Name
    ,readQuestionType.Ordinal
from [Education].[ReadQuestionType-Active] readQuestionType
order by readQuestionType.Ordinal