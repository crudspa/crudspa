create proc [ContentDesign].[SurveyPartSelectForSurvey] (
     @SessionId uniqueidentifier
    ,@SurveyId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     part.Id
    ,part.SurveyId
    ,survey.Title as SurveyTitle
    ,part.Title
    ,part.Instructions
    ,part.Ordinal
    ,(select count(1) from [Content].[SurveyQuestion-Active] where PartId = part.Id) as QuestionCount
from [Content].[SurveyPart-Active] part
    inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where part.SurveyId = @SurveyId
    and organization.Id = @organizationId
order by part.Ordinal