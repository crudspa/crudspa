create view [Content].[EmailLog-Active] as

select emailLog.Id as Id
    ,emailLog.EmailId as EmailId
    ,emailLog.RecipientId as RecipientId
    ,emailLog.RecipientEmail as RecipientEmail
    ,emailLog.Processed as Processed
    ,emailLog.Status as Status
    ,emailLog.ApiResponse as ApiResponse
from [Content].[EmailLog] emailLog
where 1=1