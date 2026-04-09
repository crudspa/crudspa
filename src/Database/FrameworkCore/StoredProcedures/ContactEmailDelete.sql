create proc [FrameworkCore].[ContactEmailDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[ContactEmail]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id