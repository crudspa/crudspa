create proc [ContentDesign].[SurveySelectForPortal] (
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
    ,survey.PortalId
    ,portal.[Key] as PortalKey
    ,survey.Title
    ,survey.Description
    ,survey.StatusId
    ,status.Name as StatusName
    ,survey.AssignmentKind
    ,(select count(1) from [Content].[SurveyPart-Active] where SurveyId = survey.Id) as PartCount
from [Content].[Survey-Active] survey
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on survey.StatusId = status.Id
where survey.PortalId = @PortalId
    and organization.Id = @organizationId
order by survey.Title