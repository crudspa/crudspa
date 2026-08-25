create proc [ContentMessaging].[OrganizationUsersSelect] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@OrganizationId uniqueidentifier
) as

set nocount on

if not exists (select 1 from [ContentMessaging].[SessionCanReadOrganization](@SessionId, @PortalId, @OrganizationId))
    throw 51000, 'Population access denied.', 1

select distinct
     [contact].[Id]
    ,[contact].[FirstName]
    ,[contact].[LastName]
    ,[contactEmail].[Email]
from [Framework].[User-Active] as [userTable]
    inner join [Framework].[Contact-Active] as [contact] on [userTable].[ContactId] = [contact].[Id]
    inner join [Framework].[ContactEmail-Active] as [contactEmail] on [contactEmail].[ContactId] = [contact].[Id]
        and [contactEmail].[Ordinal] = 0
where [userTable].[OrganizationId] = @OrganizationId
    and [userTable].[PortalId] = @PortalId
order by [contact].[LastName], [contact].[FirstName], [contact].[Id]