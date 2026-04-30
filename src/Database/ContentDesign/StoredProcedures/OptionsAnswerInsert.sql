create proc [ContentDesign].[OptionsAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Orientation int
    ,@AllowOther bit
    ,@OtherLabel nvarchar(50)
    ,@MinSelections int
    ,@MaxSelections int
    ,@Ordering int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[OptionsAnswer] (
     Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, Orientation, AllowOther, OtherLabel, MinSelections, MaxSelections, Ordering
)
values (
     @Id, @Id, @now, @SessionId, @QuestionId, @Kind, @Orientation, isnull(@AllowOther, 0), @OtherLabel, @MinSelections, @MaxSelections, @Ordering
)

commit transaction