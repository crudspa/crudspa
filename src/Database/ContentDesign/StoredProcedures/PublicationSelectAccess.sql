create proc [ContentDesign].[PublicationSelectAccess] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@GroupId uniqueidentifier
) as
set nocount on
select convert(int, case when exists (
    select 1 from Framework.[Portal-Active] portal
        inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
        inner join Framework.[Session-Active] session on session.UserId = userTable.Id
    where portal.Id = @PortalId and session.Id = @SessionId
        and (@GroupId is null or exists (select 1 from Content.[PublicationGroup-Active] groups where groups.Id = @GroupId and groups.PortalId = portal.Id))
) then 1 else 0 end)