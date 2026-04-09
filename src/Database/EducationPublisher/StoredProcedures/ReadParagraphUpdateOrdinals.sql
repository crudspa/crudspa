create proc [EducationPublisher].[ReadParagraphUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update readParagraph
set
     readParagraph.Ordinal = orderable.Ordinal
    ,readParagraph.Updated = @now
    ,readParagraph.UpdatedBy = @SessionId
from [Education].[ReadParagraph] readParagraph
    inner join @Orderables orderable on orderable.Id = readParagraph.Id
where readParagraph.Ordinal != orderable.Ordinal

commit transaction