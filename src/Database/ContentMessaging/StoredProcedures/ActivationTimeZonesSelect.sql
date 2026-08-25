create proc [ContentMessaging].[ActivationTimeZonesSelect] (
    @OrganizationIds [ContentMessaging].[GuidList] readonly
) as

set nocount on

select organization.Id, organization.TimeZoneId
from @OrganizationIds target
    inner join [Framework].[Organization-Active] organization on target.Id = organization.Id
order by organization.Id