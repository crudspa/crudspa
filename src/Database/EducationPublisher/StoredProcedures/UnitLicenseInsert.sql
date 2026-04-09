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

if not exists (
    select 1
    from [Education].[UnitLicense-Active] unitLicense
        inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unitLicense.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction