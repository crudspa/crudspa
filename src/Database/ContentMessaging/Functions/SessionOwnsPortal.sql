create function [ContentMessaging].[SessionOwnsPortal]
(
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
)
returns table
as
return
(
    select cast(1 as bit) as Allowed
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
        inner join [Framework].[Portal-Active] portal on portal.Id = @PortalId
    where session.Id = @SessionId
        and userTable.OrganizationId = portal.OwnerId
)