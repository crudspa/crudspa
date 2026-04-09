create proc [FrameworkCore].[AccessCodeUpdate] (
     @UserId uniqueidentifier
    ,@Code nvarchar(40)
    ,@Success bit output
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[AccessCode]
set Used = @now
where UserId = @UserId
    and Code = @Code
    and Expires > @now
    and Used is null

if (@@rowcount = 0) set @Success = 0
else set @Success = 1