create proc [EducationPublisher].[SegmentLicenseSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     segmentLicense.Id
    ,segmentLicense.LicenseId
    ,segmentLicense.SegmentId
    ,segment.Title as SegmentTitle
from [Framework].[SegmentLicense-Active] segmentLicense
    inner join [Framework].[License-Active] license on segmentLicense.LicenseId = license.Id
    inner join [Framework].[Segment-Active] segment on segmentLicense.SegmentId = segment.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
where segmentLicense.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId