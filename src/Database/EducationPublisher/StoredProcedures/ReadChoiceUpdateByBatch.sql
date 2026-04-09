create proc [EducationPublisher].[ReadChoiceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ReadQuestionId uniqueidentifier
    ,@Text nvarchar(max)
    ,@IsCorrect bit
    ,@ImageFileId uniqueidentifier
    ,@AudioFileId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Education].[ReadChoice]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Text = @Text
    ,IsCorrect = @IsCorrect
    ,ImageFileId = @ImageFileId
    ,AudioFileId = @AudioFileId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction