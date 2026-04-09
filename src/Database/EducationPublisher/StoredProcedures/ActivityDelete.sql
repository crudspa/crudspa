create proc [EducationPublisher].[ActivityDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update activity
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Activity] activity

where activity.Id = @Id


commit transaction