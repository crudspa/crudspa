create proc [ContentJobs].[EmailUpdateStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Status int
) as

declare @now datetimeoffset(7) = sysdatetimeoffset()

set nocount on

update email
set  email.Updated = @now
    ,email.UpdatedBy = @SessionId
    ,email.Status = @Status
    ,email.Processed = @now
from [Content].[Email] email
where email.Id = @Id