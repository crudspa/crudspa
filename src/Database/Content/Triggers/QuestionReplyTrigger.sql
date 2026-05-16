create trigger [Content].[QuestionReplyTrigger] on [Content].[QuestionReply]
    for update
as

insert [Content].[QuestionReply] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,SurveyReplyId
    ,QuestionId
    ,Submitted
    ,BoolValue
    ,TextValue
    ,HtmlValue
    ,DateValue
    ,TimeValue
    ,DateTimeValue
    ,IntegerValue
    ,DecimalValue
    ,CurrencyValue
    ,OtherBoolValue
    ,OtherTextValue
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,PostalId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.SurveyReplyId
    ,deleted.QuestionId
    ,deleted.Submitted
    ,deleted.BoolValue
    ,deleted.TextValue
    ,deleted.HtmlValue
    ,deleted.DateValue
    ,deleted.TimeValue
    ,deleted.DateTimeValue
    ,deleted.IntegerValue
    ,deleted.DecimalValue
    ,deleted.CurrencyValue
    ,deleted.OtherBoolValue
    ,deleted.OtherTextValue
    ,deleted.AudioId
    ,deleted.ImageId
    ,deleted.PdfId
    ,deleted.VideoId
    ,deleted.PostalId
from deleted