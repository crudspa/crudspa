create proc [EducationPublisher].[TrackLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@TrackId uniqueidentifier
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

if exists (
    select 1
    from [Content].[TrackLicense-Active] with (updlock, holdlock)
    where TrackId = @TrackId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Content].[TrackLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,TrackId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@TrackId
)

if not exists (
    select 1
    from [Content].[TrackLicense-Active] trackLicense
        inner join [Framework].[License-Active] license on trackLicense.LicenseId = license.Id
        inner join [Content].[Track-Active] track on trackLicense.TrackId = track.Id
        inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    where trackLicense.Id = @Id
        and license.OwnerId = @organizationId
        and portal.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction