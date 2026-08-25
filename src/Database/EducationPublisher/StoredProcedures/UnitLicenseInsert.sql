create proc [EducationPublisher].[UnitLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@UnitId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
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
    from [Education].[Unit-Active] unit
        inner join [Framework].[License-Active] license on license.Id = @LicenseId
    where unit.Id = @UnitId
        and unit.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if exists (
    select 1
    from [Education].[UnitLicense-Active] unitLicense with (updlock, holdlock)
    where unitLicense.LicenseId = @LicenseId
        and unitLicense.UnitId = @UnitId
)
begin
    rollback transaction
    raiserror('The unit is already related to this license', 16, 1)
    return
end

insert [Education].[UnitLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,UnitId
    ,AllBooks
    ,AllLessons
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@UnitId
    ,1
    ,1
)

commit transaction