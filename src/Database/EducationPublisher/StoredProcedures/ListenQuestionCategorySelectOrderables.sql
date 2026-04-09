create proc [EducationPublisher].[ListenQuestionCategorySelectOrderables] as

set nocount on
select
     listenQuestionCategory.Id
    ,listenQuestionCategory.Name as Name
    ,listenQuestionCategory.Ordinal
from [Education].[ListenQuestionCategory-Active] listenQuestionCategory
order by listenQuestionCategory.Ordinal