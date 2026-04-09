create proc [EducationPublisher].[GameSectionUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update gameSection
set
     gameSection.Ordinal = orderable.Ordinal
    ,gameSection.Updated = @now
    ,gameSection.UpdatedBy = @SessionId
from [Education].[GameSection] gameSection
    inner join @Orderables orderable on orderable.Id = gameSection.Id
where gameSection.Ordinal != orderable.Ordinal

commit transaction