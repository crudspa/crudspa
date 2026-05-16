create proc [ContentDesign].[SurveySelectNames] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
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
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where survey.PortalId = @PortalId
    and organization.Id = @organizationId
order by survey.Title