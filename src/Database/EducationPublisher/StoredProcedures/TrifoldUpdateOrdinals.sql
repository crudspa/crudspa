create proc [EducationPublisher].[TrifoldUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update trifold
set
     trifold.Ordinal = orderable.Ordinal
    ,trifold.Updated = @now
    ,trifold.UpdatedBy = @SessionId
from [Education].[Trifold] trifold
    inner join @Orderables orderable on orderable.Id = trifold.Id
where trifold.Ordinal != orderable.Ordinal

commit transaction