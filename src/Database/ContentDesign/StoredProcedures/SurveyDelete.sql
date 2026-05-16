create proc [ContentDesign].[SurveyDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
    from [Content].[Survey-Active] survey
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where survey.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update survey
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Survey] survey
where survey.Id = @Id

update part
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SurveyPart] part
where part.SurveyId = @Id

update surveyQuestion
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SurveyQuestion] surveyQuestion
    inner join [Content].[SurveyPart] part on surveyQuestion.PartId = part.Id
where part.SurveyId = @Id

commit transaction