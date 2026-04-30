create proc [ContentDisplay].[AnswerChoiceReplyInsertByBatch] (
     @SessionId uniqueidentifier
    ,@QuestionReplyId uniqueidentifier
    ,@ChoiceId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[AnswerChoiceReply] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,QuestionReplyId
    ,ChoiceId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@QuestionReplyId
    ,@ChoiceId
)

commit transaction