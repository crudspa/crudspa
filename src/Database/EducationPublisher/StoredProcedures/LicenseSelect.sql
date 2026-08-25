create proc [EducationPublisher].[LicenseSelect] (
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
     license.Id
    ,license.Name
    ,license.Description
    ,(select count(1) from [Education].[DistrictLicense-Active] where LicenseId = license.Id) as DistrictLicenseCount
    ,(select count(1) from [Education].[UnitLicense-Active] where LicenseId = license.Id) as UnitLicenseCount
    ,(select count(1) from [Framework].[SegmentLicense-Active] where LicenseId = license.Id) as SegmentLicenseCount
    ,(select count(1) from [Education].[AssessmentLicense-Active] where LicenseId = license.Id) as AssessmentLicenseCount
    ,(select count(1) from [Content].[BlogLicense-Active] where LicenseId = license.Id) as BlogLicenseCount
    ,(select count(1) from [Content].[ForumLicense-Active] where LicenseId = license.Id) as ForumLicenseCount
    ,(select count(1) from [Content].[CampaignLicense-Active] where LicenseId = license.Id) as CampaignLicenseCount
    ,(select count(1) from [Content].[TrackLicense-Active] where LicenseId = license.Id) as TrackLicenseCount
    ,(select count(1) from [Content].[SurveyLicense-Active] where LicenseId = license.Id) as SurveyLicenseCount
from [Framework].[License-Active] license
    inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
where license.Id = @Id
    and organization.Id = @organizationId

select distinct
     @Id as LicenseId
    ,segment.Id as SegmentId
    ,segment.Title as SegmentTitle
    ,convert(bit, iif(segmentLicense.Id is null, 0, 1)) as Selected
    ,segment.Ordinal
from [Framework].[Segment-Active] segment
    left join [Framework].[SegmentLicense-Active] segmentLicense on segmentLicense.SegmentId = segment.Id
        and segmentLicense.LicenseId = @Id
    inner join [Framework].[Portal-Active] portal on portal.Id = segment.PortalId
        and portal.OwnerId = @organizationId
order by segment.Ordinal