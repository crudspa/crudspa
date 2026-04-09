create proc [FrameworkJobs].[JobUpdateStatus] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Ended datetimeoffset
    ,@StatusId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Job]
set  Ended = @Ended
    ,StatusId = @StatusId
where Id = @Id