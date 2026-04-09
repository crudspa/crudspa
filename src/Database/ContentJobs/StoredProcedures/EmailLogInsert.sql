create proc [ContentJobs].[EmailLogInsert] (
     @SessionId uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@RecipientId uniqueidentifier
    ,@RecipientEmail nvarchar(75)
    ,@Status int
    ,@ApiResponse nvarchar(max)
) as

declare @Id uniqueidentifier = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[EmailLog] (
     Id
    ,Updated
    ,UpdatedBy
    ,EmailId
    ,RecipientId
    ,RecipientEmail
    ,Processed
    ,Status
    ,ApiResponse
)
values (
     @Id
    ,@now
    ,@SessionId
    ,@EmailId
    ,@RecipientId
    ,@RecipientEmail
    ,@now
    ,@Status
    ,@ApiResponse
)

commit transaction