create proc [EducationPublisher].[PublisherContactUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@UserId uniqueidentifier
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
from [Education].[PublisherContact] baseTable
    inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.Id = baseTable.Id
    inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
where baseTable.Id = @Id
    and publisher.OrganizationId = @organizationId
    and publisher.Id = @publisherId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction