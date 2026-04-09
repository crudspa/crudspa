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
from [Framework].[License-Active] license
    inner join [Framework].[Organization-Active] organization on license.OwnerId = organization.Id
where license.Id = @Id
    and organization.Id = @organizationId