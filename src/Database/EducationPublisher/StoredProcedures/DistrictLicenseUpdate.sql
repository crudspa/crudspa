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
    where districtLicense.Id = @Id
        and district.PublisherId = @publisherId
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
where baseTable.Id = @Id
    and district.PublisherId = @publisherId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (@existingDistrictId != @DistrictId)
begin

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
    where baseTable.Id = @Id
        and targetDistrict.PublisherId = @publisherId

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