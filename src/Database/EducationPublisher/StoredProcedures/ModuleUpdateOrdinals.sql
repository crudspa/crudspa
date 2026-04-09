create proc [EducationPublisher].[ModuleUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update module
set
     module.Ordinal = orderable.Ordinal
    ,module.Updated = @now
    ,module.UpdatedBy = @SessionId
from [Education].[Module] module
    inner join @Orderables orderable on orderable.Id = module.Id
where module.Ordinal != orderable.Ordinal

commit transaction