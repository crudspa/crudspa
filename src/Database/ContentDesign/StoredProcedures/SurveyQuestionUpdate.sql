create proc [ContentDesign].[SurveyQuestionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[SurveyQuestion-Active] surveyQuestion
        inner join [Content].[SurveyPart-Active] part on surveyQuestion.PartId = part.Id
        inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where surveyQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update surveyQuestion
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
from [Content].[SurveyQuestion] surveyQuestion
where surveyQuestion.Id = @Id

commit transaction