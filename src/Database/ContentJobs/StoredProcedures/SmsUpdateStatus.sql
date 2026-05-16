create proc [ContentJobs].[SmsUpdateStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Status int
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Content].[Sms]
set Status = @Status,
    Processed = @now,
    Updated = @now,
    UpdatedBy = @SessionId
where Id = @Id
    and VersionOf = Id
    and IsDeleted = 0