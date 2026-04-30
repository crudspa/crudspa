create proc [ContentDesign].[TextAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@Placeholder nvarchar(150)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[TextAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,Label = @Label
    ,Placeholder = @Placeholder
where Id = @Id

commit transaction