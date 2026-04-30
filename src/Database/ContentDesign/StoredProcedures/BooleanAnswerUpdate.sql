create proc [ContentDesign].[BooleanAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Default bit
    ,@Orientation int
    ,@TrueLabel nvarchar(250)
    ,@FalseLabel nvarchar(250)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[BooleanAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,[Default] = isnull(@Default, 0)
    ,Orientation = @Orientation
    ,TrueLabel = @TrueLabel
    ,FalseLabel = isnull(@FalseLabel, 'No')
where Id = @Id

commit transaction