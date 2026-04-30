create view [Content].[QuestionReply-Active] as

select questionReply.Id as Id
    ,questionReply.QuestionId as QuestionId
    ,questionReply.ContactId as ContactId
    ,questionReply.Submitted as Submitted
    ,questionReply.BoolValue as BoolValue
    ,questionReply.TextValue as TextValue
    ,questionReply.HtmlValue as HtmlValue
    ,questionReply.DateValue as DateValue
    ,questionReply.TimeValue as TimeValue
    ,questionReply.DateTimeValue as DateTimeValue
    ,questionReply.IntegerValue as IntegerValue
    ,questionReply.DecimalValue as DecimalValue
    ,questionReply.CurrencyValue as CurrencyValue
    ,questionReply.OtherBoolValue as OtherBoolValue
    ,questionReply.OtherTextValue as OtherTextValue
    ,questionReply.AudioId as AudioId
    ,questionReply.ImageId as ImageId
    ,questionReply.PdfId as PdfId
    ,questionReply.VideoId as VideoId
    ,questionReply.PostalId as PostalId
from [Content].[QuestionReply] questionReply
where 1=1
    and questionReply.IsDeleted = 0
    and questionReply.VersionOf = questionReply.Id