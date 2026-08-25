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

if not exists (
    select 1
    from [Education].[District-Active] district
        inner join [Education].[Publisher-Active] publisher on publisher.Id = district.PublisherId
        inner join [Framework].[License-Active] license on license.Id = @LicenseId
    where district.Id = @DistrictId
        and publisher.Id = @publisherId
        and license.OwnerId = publisher.OrganizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if exists (
    select 1
    from [Education].[DistrictLicense-Active] districtLicense with (updlock, holdlock)
    where districtLicense.LicenseId = @LicenseId
        and districtLicense.DistrictId = @DistrictId
)
begin
    rollback transaction
    raiserror('The district is already related to this license', 16, 1)
    return
end

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

commit transaction