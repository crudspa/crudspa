create proc [ContentDisplay].[QuestionReplySelectForElement] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
) as

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @binderId uniqueidentifier
declare @questionId uniqueidentifier
declare @surveyReplyId uniqueidentifier
declare @questionReplyId uniqueidentifier

select top 1
     @binderId = page.BinderId
    ,@questionId = questionElement.QuestionId
from [Content].[Element-Active] element
    inner join [Content].[QuestionElement-Active] questionElement on element.Id = questionElement.ElementId
    inner join [Content].[Section-Active] section on element.SectionId = section.Id
    inner join [Content].[Page-Active] page on section.PageId = page.Id
where element.Id = @ElementId

select top 1 @surveyReplyId = surveyReply.Id
from [Content].[SurveyReply-Active] surveyReply
where surveyReply.ContactId = @contactId
    and surveyReply.BinderId = @binderId
    and surveyReply.SurveyId is null
    and surveyReply.Terminated is null
order by surveyReply.Started desc

select top 1 @questionReplyId = questionReply.Id
from [Content].[QuestionReply-Active] questionReply
where questionReply.SurveyReplyId = @surveyReplyId
    and questionReply.QuestionId = @questionId
order by questionReply.Submitted desc

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
    left join [Framework].[UsaPostal-Active] postal on questionReply.PostalId = postal.Id
where questionReply.Id = @questionReplyId

select
     answerChoiceReply.Id
    ,answerChoiceReply.QuestionReplyId
    ,answerChoiceReply.ChoiceId
from [Content].[AnswerChoiceReply-Active] answerChoiceReply
where answerChoiceReply.QuestionReplyId = @questionReplyId