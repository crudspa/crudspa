create proc [ContentDesign].[ContactAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[ContactAnswer] (Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, Label)
values (@Id, @Id, @now, @SessionId, @QuestionId, @Kind, @Label)

commit transaction