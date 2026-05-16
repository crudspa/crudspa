create proc [ContentDesign].[SurveyPartUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update part
set
     part.Ordinal = orderable.Ordinal
    ,part.Updated = @now
    ,part.UpdatedBy = @SessionId
from [Content].[SurveyPart] part
    inner join @Orderables orderable on orderable.Id = part.Id
where part.Ordinal != orderable.Ordinal

commit transaction