create proc [EducationPublisher].[VocabPartUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update vocabPart
set
     vocabPart.Ordinal = orderable.Ordinal
    ,vocabPart.Updated = @now
    ,vocabPart.UpdatedBy = @SessionId
from [Education].[VocabPart] vocabPart
    inner join @Orderables orderable on orderable.Id = vocabPart.Id
where vocabPart.Ordinal != orderable.Ordinal

commit transaction