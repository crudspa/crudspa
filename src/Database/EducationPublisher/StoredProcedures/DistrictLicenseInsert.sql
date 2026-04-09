create proc [EducationPublisher].[DistrictLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@DistrictId uniqueidentifier
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

insert [Education].[DistrictLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,DistrictId
    ,AllSchools
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@DistrictId
    ,1
)

if not exists (
    select 1
    from [Education].[DistrictLicense-Active] districtLicense
        inner join [Education].[District-Active] district on districtLicense.DistrictId = district.Id
    where districtLicense.Id = @Id
        and district.PublisherId = @publisherId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction