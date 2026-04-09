create proc [EducationPublisher].[ListenQuestionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update listenQuestion
set
     listenQuestion.Ordinal = orderable.Ordinal
    ,listenQuestion.Updated = @now
    ,listenQuestion.UpdatedBy = @SessionId
from [Education].[ListenQuestion] listenQuestion
    inner join @Orderables orderable on orderable.Id = listenQuestion.Id
where listenQuestion.Ordinal != orderable.Ordinal

commit transaction