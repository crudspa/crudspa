create proc [EducationProvider].[ProviderContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@UserId uniqueidentifier
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,UserId = @UserId
from [Education].[ProviderContact] baseTable
    inner join [Education].[ProviderContact-Active] providerContact on providerContact.Id = baseTable.Id
    inner join [Education].[Provider-Active] provider on providerContact.ProviderId = provider.Id
where baseTable.Id = @Id
    and provider.Id = @providerId
    and provider.OrganizationId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction