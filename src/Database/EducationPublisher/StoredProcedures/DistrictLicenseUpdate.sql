create proc [EducationPublisher].[DistrictLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@DistrictId uniqueidentifier
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
declare @existingDistrictId uniqueidentifier = (
    select top 1 districtLicense.DistrictId
    from [Education].[DistrictLicense-Active] districtLicense
        inner join [Education].[District-Active] district on districtLicense.DistrictId = district.Id
        inner join [Education].[Publisher-Active] publisher on publisher.Id = district.PublisherId
        inner join [Framework].[License-Active] license on license.Id = districtLicense.LicenseId
    where districtLicense.Id = @Id
        and district.PublisherId = @publisherId
        and license.OwnerId = publisher.OrganizationId
)

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[DistrictLicense] baseTable
    inner join [Education].[DistrictLicense-Active] districtLicense on districtLicense.Id = baseTable.Id
    inner join [Education].[District-Active] district on districtLicense.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on publisher.Id = district.PublisherId
    inner join [Framework].[License-Active] license on license.Id = districtLicense.LicenseId
where baseTable.Id = @Id
    and district.PublisherId = @publisherId
    and license.OwnerId = publisher.OrganizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (@existingDistrictId != @DistrictId)
begin

    if exists (
        select 1
        from [Education].[DistrictLicense-Active] existing with (updlock, holdlock)
            inner join [Education].[DistrictLicense-Active] updating on updating.Id = @Id
        where existing.LicenseId = updating.LicenseId
            and existing.DistrictId = @DistrictId
            and existing.Id != @Id
    )
    begin
        rollback transaction
        raiserror('The district is already related to this license', 16, 1)
        return
    end

    update baseTable
    set
         Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,DistrictId = @DistrictId
        ,AllSchools = 1
    from [Education].[DistrictLicense] baseTable
        inner join [Education].[DistrictLicense-Active] districtLicense on districtLicense.Id = baseTable.Id
        inner join [Education].[District-Active] targetDistrict on targetDistrict.Id = @DistrictId
        inner join [Education].[Publisher-Active] publisher on publisher.Id = targetDistrict.PublisherId
        inner join [Framework].[License-Active] license on license.Id = districtLicense.LicenseId
    where baseTable.Id = @Id
        and targetDistrict.PublisherId = @publisherId
        and license.OwnerId = publisher.OrganizationId

    if @@rowcount = 0
    begin
        rollback transaction
        raiserror('Tenancy check failed', 16, 1)
        return
    end

    update [Education].[DistrictLicenseSchool]
    set IsDeleted = 1
         ,Updated = @now
         ,UpdatedBy = @SessionId
    where DistrictLicenseId = @Id
         and IsDeleted = 0
         and VersionOf = Id
end

commit transaction