create proc [ContentDesign].[SmsUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
    ,@Body nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TemplateId = @TemplateId
    ,Send = @Send
    ,Body = @Body
from [Content].[Sms] baseTable
    inner join [Content].[Sms-Active] sms on sms.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction