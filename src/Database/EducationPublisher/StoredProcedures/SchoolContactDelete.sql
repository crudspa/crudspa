create proc [EducationPublisher].[SchoolContactDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

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
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[SchoolContact] baseTable
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = baseTable.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where baseTable.Id = @Id
    and publisher.Id = @publisherId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction