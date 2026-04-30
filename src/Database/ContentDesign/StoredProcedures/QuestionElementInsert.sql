create proc [ContentDesign].[QuestionElementInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[QuestionElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,QuestionId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@QuestionId
)

commit transaction