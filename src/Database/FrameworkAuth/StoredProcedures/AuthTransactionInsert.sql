create proc [FrameworkAuth].[AuthTransactionInsert] (
     @Id uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Audience nvarchar(25)
    ,@ReturnPath nvarchar(500)
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

insert [Framework].[AuthTransaction] (
     Id
    ,Created
    ,Expires
    ,Provider
    ,Audience
    ,ReturnPath
)
values (
     @Id
    ,@now
    ,dateadd(minute, 10, @now)
    ,@Provider
    ,@Audience
    ,@ReturnPath
)

insert [Framework].[AuthEvent] (
     Id
    ,Created
    ,CorrelationId
    ,Type
    ,Outcome
    ,Provider
    ,Audience
)
values (
     newid()
    ,@now
    ,@Id
    ,N'auth-started'
    ,N'succeeded'
    ,@Provider
    ,@Audience
)

commit transaction