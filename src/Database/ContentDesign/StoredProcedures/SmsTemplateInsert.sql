create proc [ContentDesign].[SmsTemplateInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@Title nvarchar(75)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[SmsTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,Title
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@Title
    ,@Body
)

commit transaction