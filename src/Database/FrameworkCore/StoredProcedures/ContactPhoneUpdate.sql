create proc [FrameworkCore].[ContactPhoneUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Phone nvarchar(10)
    ,@Extension nvarchar(10)
    ,@SupportsSms bit
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[ContactPhone]
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Phone = @Phone
    ,Extension = @Extension
    ,SupportsSms = @SupportsSms
    ,Ordinal = @Ordinal
where Id = @Id