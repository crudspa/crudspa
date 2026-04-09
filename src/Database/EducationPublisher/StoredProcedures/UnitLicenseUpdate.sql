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
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if (@existingUnitId != @UnitId)
begin

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
        inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where baseTable.Id = @Id
        and organization.Id = @organizationId

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