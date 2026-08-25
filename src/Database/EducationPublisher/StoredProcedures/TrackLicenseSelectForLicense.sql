create proc [EducationPublisher].[TrackLicenseSelectForLicense] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     trackLicense.Id
    ,trackLicense.LicenseId
    ,trackLicense.TrackId
    ,track.Title as TrackTitle
from [Content].[TrackLicense-Active] trackLicense
    inner join [Framework].[License-Active] license on trackLicense.LicenseId = license.Id
    inner join [Content].[Track-Active] track on trackLicense.TrackId = track.Id
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
where trackLicense.LicenseId = @LicenseId
    and license.OwnerId = @organizationId
    and portal.OwnerId = @organizationId