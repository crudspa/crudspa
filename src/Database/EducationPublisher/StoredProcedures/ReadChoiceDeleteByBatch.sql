create proc [EducationPublisher].[ReadChoiceDeleteByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set xact_abort on
set nocount on
begin transaction

update [Education].[ReadChoice]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id

commit transaction