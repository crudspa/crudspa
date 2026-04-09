create proc [EducationPublisher].[DistrictLicenseUpdateRelations] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@AllSchools bit
    ,@Schools Framework.IdList readonly
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @districtId uniqueidentifier = (
    select top 1 districtLicense.DistrictId
    from [Education].[DistrictLicense-Active] districtLicense
        inner join [Education].[District-Active] district on districtLicense.DistrictId = district.Id
    where districtLicense.Id = @Id
        and district.PublisherId = @publisherId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if @districtId is null
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update baseTable
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,AllSchools = @AllSchools
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

if (@AllSchools = 0)
begin
    update [Education].[DistrictLicenseSchool]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where DistrictLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
        and not exists (select 1 from @Schools where Id = SchoolId)

    insert [Education].[DistrictLicenseSchool] (Id, VersionOf, Updated, UpdatedBy, DistrictLicenseId, SchoolId)
    select guid.NewId, guid.NewId, @now, @SessionId, @Id, school.Id
    from @Schools requestedSchool
        inner join [Education].[School-Active] school on school.Id = requestedSchool.Id
    cross apply (select newid() as NewId) guid
    where school.DistrictId = @districtId
        and not exists (
        select 1 from [Education].[DistrictLicenseSchool-Active]
        where DistrictLicenseId = @Id and SchoolId = school.Id
    )
end
else
begin
    update [Education].[DistrictLicenseSchool]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where DistrictLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
end

commit transaction