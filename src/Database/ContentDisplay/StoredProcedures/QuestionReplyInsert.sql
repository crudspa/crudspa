create proc [ContentDisplay].[QuestionReplyInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Submitted datetimeoffset
    ,@BoolValue bit
    ,@TextValue nvarchar(max)
    ,@HtmlValue nvarchar(max)
    ,@DateValue date
    ,@TimeValue time
    ,@DateTimeValue datetimeoffset
    ,@IntegerValue int
    ,@DecimalValue real
    ,@CurrencyValue real
    ,@OtherBoolValue bit
    ,@OtherTextValue nvarchar(150)
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@PostalId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
set xact_abort on
begin transaction

if (@contactId is not null)
begin

insert [Content].[QuestionReply] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,QuestionId
    ,ContactId
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
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@QuestionId
    ,@contactId
    ,isnull(@Submitted, @now)
    ,@BoolValue
    ,@TextValue
    ,@HtmlValue
    ,@DateValue
    ,@TimeValue
    ,@DateTimeValue
    ,@IntegerValue
    ,@DecimalValue
    ,@CurrencyValue
    ,isnull(@OtherBoolValue, 0)
    ,@OtherTextValue
    ,@AudioId
    ,@ImageId
    ,@PdfId
    ,@VideoId
    ,@PostalId
)

end

commit transaction