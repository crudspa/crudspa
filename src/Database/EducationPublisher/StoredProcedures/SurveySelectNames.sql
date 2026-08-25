create proc [EducationPublisher].[SurveySelectNames] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     survey.Id
    ,survey.Title as Name
from [Content].[Survey-Active] survey
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by survey.Title