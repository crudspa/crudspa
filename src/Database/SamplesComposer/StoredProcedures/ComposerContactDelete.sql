create proc [SamplesComposer].[ComposerContactDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Samples].[ComposerContact] baseTable
    inner join [Samples].[ComposerContact-Active] composerContact on composerContact.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction