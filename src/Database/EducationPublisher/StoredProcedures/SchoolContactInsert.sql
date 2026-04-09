create proc [EducationPublisher].[SchoolContactInsert] (
     @SessionId uniqueidentifier
    ,@SchoolId uniqueidentifier
    ,@TitleId uniqueidentifier
    ,@TestAccount bit
    ,@Treatment bit
    ,@ContactId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Id uniqueidentifier output
) as

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

insert [Education].[SchoolContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SchoolId
    ,TitleId
    ,TestAccount
    ,Treatment
    ,ContactId
    ,UserId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SchoolId
    ,@TitleId
    ,@TestAccount
    ,@Treatment
    ,@ContactId
    ,@UserId
)

if not exists (
    select 1
    from [Education].[SchoolContact-Active] schoolContact
        inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where schoolContact.Id = @Id
        and publisher.Id = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction