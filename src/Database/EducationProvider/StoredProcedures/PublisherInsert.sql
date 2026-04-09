create proc [EducationProvider].[PublisherInsert] (
     @SessionId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Education].[Publisher] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,OrganizationId
    ,ProviderId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@OrganizationId
    ,@providerId
)

if not exists (
    select 1
    from [Education].[Publisher-Active] publisher
        inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
    where publisher.Id = @Id
        and provider.Id = @providerId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction