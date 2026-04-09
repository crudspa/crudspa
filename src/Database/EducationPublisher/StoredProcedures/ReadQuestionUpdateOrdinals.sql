create proc [EducationPublisher].[ReadQuestionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update readQuestion
set
     readQuestion.Ordinal = orderable.Ordinal
    ,readQuestion.Updated = @now
    ,readQuestion.UpdatedBy = @SessionId
from [Education].[ReadQuestion] readQuestion
    inner join @Orderables orderable on orderable.Id = readQuestion.Id
where readQuestion.Ordinal != orderable.Ordinal

commit transaction