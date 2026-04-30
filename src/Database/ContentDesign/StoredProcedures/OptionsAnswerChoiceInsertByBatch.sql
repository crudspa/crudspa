create proc [ContentDesign].[OptionsAnswerChoiceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@OptionsAnswerId uniqueidentifier
    ,@Text nvarchar(max)
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[OptionsAnswerChoice] (
     Id, VersionOf, Updated, UpdatedBy, OptionsAnswerId, Text, Ordinal
)
values (
     @Id, @Id, @now, @SessionId, @OptionsAnswerId, @Text, @Ordinal
)

commit transaction