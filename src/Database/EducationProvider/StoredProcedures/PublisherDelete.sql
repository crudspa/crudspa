create proc [EducationProvider].[PublisherDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Publisher] baseTable
    inner join [Education].[Publisher-Active] publisher on publisher.Id = baseTable.Id
    inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
where baseTable.Id = @Id
    and provider.Id = @providerId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction