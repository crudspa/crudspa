create proc [FrameworkCore].[ContactEmailUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Email nvarchar(75)
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[ContactEmail]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,Email = @Email
    ,Ordinal = @Ordinal
where Id = @Id