create proc [ContentDesign].[BooleanAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Default bit
    ,@Orientation int
    ,@TrueLabel nvarchar(250)
    ,@FalseLabel nvarchar(250)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[BooleanAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,QuestionId
    ,Kind
    ,[Default]
    ,Orientation
    ,TrueLabel
    ,FalseLabel
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@QuestionId
    ,@Kind
    ,isnull(@Default, 0)
    ,@Orientation
    ,@TrueLabel
    ,isnull(@FalseLabel, 'No')
)

commit transaction