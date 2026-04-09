create proc [FrameworkCore].[OrganizationDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update organization
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Framework].[Organization] organization
where organization.Id = @Id

commit transaction