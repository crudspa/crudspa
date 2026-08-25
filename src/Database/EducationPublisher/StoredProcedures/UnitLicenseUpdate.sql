create proc [EducationPublisher].[UnitLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@UnitId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()
declare @existingUnitId uniqueidentifier = (select UnitId from [Education].[UnitLicense] where Id = @Id)

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[UnitLicense] baseTable
    inner join [Education].[UnitLicense-Active] unitLicense on unitLicense.Id = baseTable.Id
    inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
    inner join [Framework].[License-Active] license on license.Id = unitLicense.LicenseId
where baseTable.Id = @Id
    and unit.OwnerId = @organizationId
    and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (@existingUnitId != @UnitId)
begin

    if exists (
        select 1
        from [Education].[UnitLicense-Active] existing with (updlock, holdlock)
            inner join [Education].[UnitLicense-Active] updating on updating.Id = @Id
        where existing.LicenseId = updating.LicenseId
            and existing.UnitId = @UnitId
            and existing.Id != @Id
    )
    begin
        rollback transaction
        raiserror('The unit is already related to this license', 16, 1)
        return
    end

    update baseTable
    set
         Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,UnitId = @UnitId
        ,AllBooks = 1
        ,AllLessons = 1
    from [Education].[UnitLicense] baseTable
        inner join [Education].[UnitLicense-Active] unitLicense on unitLicense.Id = baseTable.Id
        inner join [Education].[Unit-Active] unit on unit.Id = @UnitId
        inner join [Framework].[License-Active] license on license.Id = unitLicense.LicenseId
    where baseTable.Id = @Id
        and unit.OwnerId = @organizationId
        and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

    update [Education].[UnitLicenseBook]
    set IsDeleted = 1
         ,Updated = @now
         ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
         and IsDeleted = 0
         and VersionOf = Id
    update [Education].[UnitLicenseLesson]
    set IsDeleted = 1
         ,Updated = @now
         ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
         and IsDeleted = 0
         and VersionOf = Id
end

commit transaction