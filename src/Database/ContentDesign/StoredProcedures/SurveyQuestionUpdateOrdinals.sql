create proc [ContentDesign].[SurveyQuestionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update surveyQuestion
set
     surveyQuestion.Ordinal = orderable.Ordinal
    ,surveyQuestion.Updated = @now
    ,surveyQuestion.UpdatedBy = @SessionId
from [Content].[SurveyQuestion] surveyQuestion
    inner join @Orderables orderable on orderable.Id = surveyQuestion.Id
where surveyQuestion.Ordinal != orderable.Ordinal

commit transaction