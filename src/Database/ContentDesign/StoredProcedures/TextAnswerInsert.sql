create proc [ContentDesign].[TextAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@Placeholder nvarchar(150)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[TextAnswer] (
     Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, Label, Placeholder
)
values (
     @Id, @Id, @now, @SessionId, @QuestionId, @Kind, @Label, @Placeholder
)

commit transaction