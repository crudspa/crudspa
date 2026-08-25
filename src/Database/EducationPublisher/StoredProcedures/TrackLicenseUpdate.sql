create proc [EducationPublisher].[TrackLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TrackId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Content].[TrackLicense-Active] with (updlock, holdlock)
    where Id != @Id
        and TrackId = @TrackId
        and LicenseId = (select LicenseId from [Content].[TrackLicense-Active] where Id = @Id)
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TrackId = @TrackId
from [Content].[TrackLicense] baseTable
    inner join [Content].[TrackLicense-Active] trackLicense on trackLicense.Id = baseTable.Id
    inner join [Framework].[License-Active] license on trackLicense.LicenseId = license.Id
    inner join [Content].[Track-Active] track on trackLicense.TrackId = track.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where baseTable.Id = @Id
    and license.OwnerId = @organizationId
    and portal.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction