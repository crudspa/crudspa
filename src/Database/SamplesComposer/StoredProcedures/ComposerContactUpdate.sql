create proc [SamplesComposer].[ComposerContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,UserId = @UserId
from [Samples].[ComposerContact] baseTable
    inner join [Samples].[ComposerContact-Active] composerContact on composerContact.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction