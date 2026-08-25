create proc [FrameworkCore].[AccessCodeInsert] (
     @SessionId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Code nvarchar(40)
    ,@Expires datetimeoffset(7)
) as

set xact_abort on

begin transaction

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[AccessCode]
set Used = @now
where UserId = @UserId
    and PortalId = @PortalId
    and Used is null
    and Expires > @now

insert [Framework].[AccessCode] (
    Id
    ,SessionId
    ,UserId
    ,PortalId
    ,Code
    ,Expires
)
values (
    newid()
    ,@SessionId
    ,@UserId
    ,@PortalId
    ,@Code
    ,@Expires
)

commit transaction