create proc [EducationPublisher].[PublisherContactInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
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
    ,ContactId
    ,UserId
    ,PublisherId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ContactId
    ,@UserId
    ,@publisherId
)

if not exists (
    select 1
    from [Education].[PublisherContact-Active] publisherContact
        inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
    where publisherContact.Id = @Id
        and publisher.OrganizationId = @organizationId
        and publisher.Id = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction