create proc [FrameworkCore].[SessionIsValidForSignIn] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Valid bit output
) as

if exists (
    select 1
    from [Framework].[Session-Active] session
    where session.Id = @SessionId
        and session.PortalId = @PortalId
        and session.Ended is null
        and session.UserId is null
        and session.UserAdded is null
)
begin
    set @Valid = 1
end
else
begin
    set @Valid = 0
end