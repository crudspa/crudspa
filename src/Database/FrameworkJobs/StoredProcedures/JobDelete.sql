create proc [FrameworkJobs].[JobDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Job]
set IsDeleted = 1
where Id = @Id