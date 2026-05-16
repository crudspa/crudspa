create proc [ContentDesign].[SurveyQuestionInsert] (
     @SessionId uniqueidentifier
    ,@PartId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()
declare @ordinal int = (select count(1) from [Content].[SurveyQuestion-Active] where PartId = @PartId)

set nocount on
set xact_abort on
begin transaction

insert [Content].[SurveyQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PartId
    ,QuestionId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PartId
    ,@QuestionId
    ,@ordinal
)

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

commit transaction