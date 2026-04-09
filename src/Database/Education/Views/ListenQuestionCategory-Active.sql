create view [Education].[ListenQuestionCategory-Active] as

select listenQuestionCategory.Id as Id
    ,listenQuestionCategory.Name as Name
    ,listenQuestionCategory.Ordinal as Ordinal
from [Education].[ListenQuestionCategory] listenQuestionCategory
where 1=1
    and listenQuestionCategory.IsDeleted = 0