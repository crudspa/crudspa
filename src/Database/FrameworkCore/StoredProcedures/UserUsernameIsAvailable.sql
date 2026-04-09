create proc [FrameworkCore].[UserUsernameIsAvailable] (
     @Username nvarchar(75)
    ,@PortalId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Available bit output
) as

set @Available = case
    when exists (
        select 1
        from [Framework].[User-Active]
        where Username = @Username
          and PortalId = @PortalId
          and (@UserId is null or Id != @UserId)
    )
    then 0 else 1 end