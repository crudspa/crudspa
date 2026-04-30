create proc [ContentDesign].[QuestionElementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@QuestionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[QuestionElement]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ElementId = @ElementId
    ,QuestionId = @QuestionId
where Id = @Id

commit transaction