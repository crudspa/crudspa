create proc [ContentDesign].[ForumPortalAuthorize] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as
begin
    set nocount on;

    select convert(bit, case when exists (
        select 1
        from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        inner join [Framework].[Portal-Active] portal on portal.Id = @PortalId
            and portal.OwnerId = userTable.OrganizationId
        where session.Id = @SessionId and session.Ended is null
            and exists (
                select 1
                from [Framework].[PortalPaneType-Active] portalPaneType
                where portalPaneType.PortalId = portal.Id
                    and portalPaneType.TypeId = '730eb0d1-2e7d-47dc-8b50-7434bfb25b64'
            )
    ) then 1 else 0 end);
end