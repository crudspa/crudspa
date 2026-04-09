create proc [EducationPublisher].[ListenChoiceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ListenQuestionId uniqueidentifier
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

update [Education].[ListenChoice]
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