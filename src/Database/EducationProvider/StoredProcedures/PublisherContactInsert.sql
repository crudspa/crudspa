create proc [EducationProvider].[PublisherContactInsert] (
     @SessionId uniqueidentifier
    ,@PublisherId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
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

insert [Education].[PublisherContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PublisherId
    ,ContactId
    ,UserId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PublisherId
    ,@ContactId
    ,@UserId
)

if not exists (
    select 1
    from [Education].[PublisherContact-Active] publisherContact
        inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
        inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
    where publisherContact.Id = @Id
        and provider.Id = @providerId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction