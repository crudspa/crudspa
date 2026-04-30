create proc [ContentDesign].[OptionsAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Orientation int
    ,@AllowOther bit
    ,@OtherLabel nvarchar(50)
    ,@MinSelections int
    ,@MaxSelections int
    ,@Ordering int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[OptionsAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,Orientation = @Orientation
    ,AllowOther = isnull(@AllowOther, 0)
    ,OtherLabel = @OtherLabel
    ,MinSelections = @MinSelections
    ,MaxSelections = @MaxSelections
    ,Ordering = @Ordering
where Id = @Id

commit transaction