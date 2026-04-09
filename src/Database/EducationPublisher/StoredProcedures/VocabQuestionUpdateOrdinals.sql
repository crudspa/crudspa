create proc [EducationPublisher].[VocabQuestionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update vocabQuestion
set
     vocabQuestion.Ordinal = orderable.Ordinal
    ,vocabQuestion.Updated = @now
    ,vocabQuestion.UpdatedBy = @SessionId
from [Education].[VocabQuestion] vocabQuestion
    inner join @Orderables orderable on orderable.Id = vocabQuestion.Id
where vocabQuestion.Ordinal != orderable.Ordinal

commit transaction