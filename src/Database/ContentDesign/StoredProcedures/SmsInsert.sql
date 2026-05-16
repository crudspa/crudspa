create proc [ContentDesign].[SmsInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[Sms] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,TemplateId
    ,Send
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@TemplateId
    ,@Send
    ,@Body
)

commit transaction