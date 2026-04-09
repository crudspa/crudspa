create proc [EducationPublisher].[SchoolInsert] (
     @SessionId uniqueidentifier
    ,@DistrictId uniqueidentifier
    ,@Key nvarchar(100)
    ,@CommunityId uniqueidentifier
    ,@Treatment bit
    ,@OrganizationId uniqueidentifier
    ,@AddressId uniqueidentifier
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

insert [Education].[School] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,DistrictId
    ,[Key]
    ,CommunityId
    ,Treatment
    ,OrganizationId
    ,AddressId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@DistrictId
    ,@Key
    ,@CommunityId
    ,@Treatment
    ,@OrganizationId
    ,@AddressId
)

if not exists (
    select 1
    from [Education].[School-Active] school
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
        inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    where school.Id = @Id
        and publisher.Id = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction