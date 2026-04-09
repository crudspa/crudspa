create proc [EducationProvider].[PublisherSelectRoleNames] (
     @SessionId uniqueidentifier
    ,@PublisherId uniqueidentifier
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     role.Id
    ,role.Name
from [Framework].[Role-Active] role
    inner join [Framework].[Organization-Active] organization on role.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on publisher.OrganizationId = organization.Id
    inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
where 1 = 1
    and publisher.Id = @PublisherId
    and provider.Id = @providerId
order by role.Name