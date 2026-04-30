create proc [ContentDesign].[QuestionInsert] (
     @SessionId uniqueidentifier
    ,@Text nvarchar(max)
    ,@AnswerTypeId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[Question] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Text
    ,AnswerTypeId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Text
    ,@AnswerTypeId
)

commit transaction