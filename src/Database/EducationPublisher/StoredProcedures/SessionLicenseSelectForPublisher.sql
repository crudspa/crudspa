create proc [EducationPublisher].[SessionLicenseSelectForPublisher] (
     @SessionId uniqueidentifier
) as

set nocount on

select license.Id
from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    inner join [Framework].[License-Active] license on license.OwnerId = userTable.OrganizationId
where session.Id = @SessionId
    and session.Ended is null
order by license.Id