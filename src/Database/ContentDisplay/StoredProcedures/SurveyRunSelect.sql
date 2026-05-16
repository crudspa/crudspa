create proc [ContentDisplay].[SurveyRunSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

declare @now datetimeoffset = sysdatetimeoffset()
declare @portalId uniqueidentifier
declare @contactId uniqueidentifier
declare @surveyReplyId uniqueidentifier

select top 1
     @portalId = session.PortalId
    ,@contactId = userTable.ContactId
from [Framework].[Session-Active] session
    left join [Framework].[User-Active] userTable on session.UserId = userTable.Id
where session.Id = @SessionId

set nocount on
set xact_abort on
begin transaction

if (@contactId is not null
    and exists (
        select 1
        from [Content].[Survey-Active] survey
        where survey.Id = @Id
            and survey.PortalId = @portalId
            and survey.StatusId = @ContentStatusComplete
    ))
begin
    select top 1 @surveyReplyId = surveyReply.Id
    from [Content].[SurveyReply-Active] surveyReply
    where surveyReply.SurveyId = @Id
        and surveyReply.ContactId = @contactId
        and surveyReply.BinderId is null
        and surveyReply.Terminated is null
    order by
         case when surveyReply.Completed is null then 1 else 0 end
        ,surveyReply.Started desc

    if (@surveyReplyId is null)
    begin
        set @surveyReplyId = newid()

        insert [Content].[SurveyReply] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,SurveyId
            ,BinderId
            ,ContactId
            ,Started
        )
        values (
             @surveyReplyId
            ,@surveyReplyId
            ,@now
            ,@SessionId
            ,@Id
            ,null
            ,@contactId
            ,@now
        )
    end
end

commit transaction

create table #Questions (
     Id uniqueidentifier not null primary key
    ,QuestionId uniqueidentifier not null
)

insert #Questions (Id, QuestionId)
select surveyQuestion.Id, surveyQuestion.QuestionId
from [Content].[SurveyQuestion-Active] surveyQuestion
    inner join [Content].[SurveyPart-Active] part on surveyQuestion.PartId = part.Id
    inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
where survey.Id = @Id
    and survey.PortalId = @portalId
    and survey.StatusId = @ContentStatusComplete

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
    inner join [Framework].[ContentStatus-Active] status on survey.StatusId = status.Id
where survey.Id = @Id
    and survey.PortalId = @portalId
    and survey.StatusId = @ContentStatusComplete

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
where survey.Id = @Id
    and survey.PortalId = @portalId
    and survey.StatusId = @ContentStatusComplete
order by part.Ordinal

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
order by part.Ordinal, surveyQuestion.Ordinal

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

select
     surveyReply.Id
    ,surveyReply.SurveyId
    ,surveyReply.BinderId
    ,surveyReply.ContactId
    ,surveyReply.Started
    ,surveyReply.Completed
    ,surveyReply.Terminated
from [Content].[SurveyReply-Active] surveyReply
where surveyReply.Id = @surveyReplyId
    and surveyReply.ContactId = @contactId

select
     questionReply.Id
    ,questionReply.SurveyReplyId
    ,questionReply.QuestionId
    ,questionReply.Submitted
    ,questionReply.BoolValue
    ,questionReply.TextValue
    ,questionReply.HtmlValue
    ,questionReply.DateValue
    ,questionReply.TimeValue
    ,questionReply.DateTimeValue
    ,questionReply.IntegerValue
    ,questionReply.DecimalValue
    ,questionReply.CurrencyValue
    ,questionReply.OtherBoolValue
    ,questionReply.OtherTextValue
    ,questionReply.AudioId
    ,questionReply.ImageId
    ,questionReply.PdfId
    ,questionReply.VideoId
    ,questionReply.PostalId
    ,postal.RecipientName
    ,postal.BusinessName
    ,postal.StreetAddress
    ,postal.City
    ,postal.StateId
    ,postal.PostalCode
from [Content].[QuestionReply-Active] questionReply
    inner join #Questions selected on questionReply.QuestionId = selected.QuestionId
    left join [Framework].[UsaPostal-Active] postal on questionReply.PostalId = postal.Id
where questionReply.SurveyReplyId = @surveyReplyId

select
     answerChoiceReply.Id
    ,answerChoiceReply.QuestionReplyId
    ,answerChoiceReply.ChoiceId
from [Content].[AnswerChoiceReply-Active] answerChoiceReply
    inner join [Content].[QuestionReply-Active] questionReply on answerChoiceReply.QuestionReplyId = questionReply.Id
where questionReply.SurveyReplyId = @surveyReplyId