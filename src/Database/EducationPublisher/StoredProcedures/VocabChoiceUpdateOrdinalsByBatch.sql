create proc [EducationPublisher].[VocabChoiceUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update vocabChoice
set
    vocabChoice.Ordinal = orderable.Ordinal
    ,vocabChoice.Updated = @now
    ,vocabChoice.UpdatedBy = @SessionId
from [Education].[VocabChoice] vocabChoice
    inner join @Orderables orderable on orderable.Id = vocabChoice.Id
where vocabChoice.Ordinal != orderable.Ordinal

commit transaction