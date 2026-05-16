create proc [ContentDesign].[SurveyPartDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @surveyId uniqueidentifier = (select top 1 SurveyId from [Content].[SurveyPart-Active] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Content].[SurveyPart-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Content].[SurveyPart-Active] part
        inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where part.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update surveyQuestion
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SurveyQuestion] surveyQuestion
where surveyQuestion.PartId = @Id

update part
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SurveyPart] part
where part.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Content].[SurveyPart] baseTable
    inner join [Content].[SurveyPart-Active] part on part.Id = baseTable.Id
where part.SurveyId = @surveyId
    and part.Ordinal > @oldOrdinal

commit transaction