create proc [ContentDesign].[SurveyQuestionSelectForPart] (
     @SessionId uniqueidentifier
    ,@PartId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

create table #Questions (
     Id uniqueidentifier not null primary key
    ,QuestionId uniqueidentifier not null
)

insert #Questions (Id, QuestionId)
select surveyQuestion.Id, surveyQuestion.QuestionId
from [Content].[SurveyQuestion-Active] surveyQuestion
    inner join [Content].[SurveyPart-Active] part on surveyQuestion.PartId = part.Id
    inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where surveyQuestion.PartId = @PartId
    and organization.Id = @organizationId

select
     surveyQuestion.Id
    ,surveyQuestion.PartId
    ,part.Title as PartTitle
    ,surveyQuestion.QuestionId
    ,question.Text
    ,question.AnswerTypeId
    ,answerType.Name as AnswerTypeName
    ,answerType.DesignView as AnswerTypeDesignView
    ,answerType.DisplayView as AnswerTypeDisplayView
    ,surveyQuestion.Ordinal
from #Questions selected
    inner join [Content].[SurveyQuestion-Active] surveyQuestion on selected.Id = surveyQuestion.Id
    inner join [Content].[SurveyPart-Active] part on surveyQuestion.PartId = part.Id
    inner join [Content].[Question-Active] question on surveyQuestion.QuestionId = question.Id
    inner join [Content].[AnswerType-Active] answerType on question.AnswerTypeId = answerType.Id
order by surveyQuestion.Ordinal

select answer.Id, answer.QuestionId, answer.Kind, answer.[Default], answer.Orientation, answer.TrueLabel, answer.FalseLabel
from [Content].[BooleanAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind, answer.Label
from [Content].[ContactAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind, answer.Label, answer.DateMin, answer.DateMax, answer.TimeMin, answer.TimeMax, answer.DateTimeMin, answer.DateTimeMax
from [Content].[DateAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind
from [Content].[FileAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind, answer.Label, answer.IntegerMin, answer.IntegerMax, answer.DecimalMin, answer.DecimalMax, answer.CurrencyMin, answer.CurrencyMax
from [Content].[NumberAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind, answer.Orientation, answer.AllowOther, answer.OtherLabel, answer.MinSelections, answer.MaxSelections, answer.Ordering
from [Content].[OptionsAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select choice.Id, choice.OptionsAnswerId, choice.Text, choice.Ordinal
from [Content].[OptionsAnswerChoice-Active] choice
    inner join [Content].[OptionsAnswer-Active] answer on choice.OptionsAnswerId = answer.Id
    inner join #Questions selected on answer.QuestionId = selected.QuestionId
order by choice.Ordinal

select answer.Id, answer.QuestionId, answer.Kind, answer.RatingKind, answer.LikertKind, answer.RatingMin, answer.RatingMax, answer.Ordering
from [Content].[ScaleAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId

select answer.Id, answer.QuestionId, answer.Kind, answer.Label, answer.Placeholder
from [Content].[TextAnswer-Active] answer
    inner join #Questions selected on answer.QuestionId = selected.QuestionId