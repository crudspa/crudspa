create proc [ContentDesign].[FileAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[FileAnswer] (Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind)
values (@Id, @Id, @now, @SessionId, @QuestionId, @Kind)

commit transaction