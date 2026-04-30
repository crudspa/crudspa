create proc [ContentDesign].[OptionsAnswerChoiceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@OptionsAnswerId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[OptionsAnswerChoice]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,OptionsAnswerId = @OptionsAnswerId
    ,Text = @Text
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction