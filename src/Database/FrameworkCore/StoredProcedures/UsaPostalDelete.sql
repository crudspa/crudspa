create proc [FrameworkCore].[UsaPostalDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[UsaPostal]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id