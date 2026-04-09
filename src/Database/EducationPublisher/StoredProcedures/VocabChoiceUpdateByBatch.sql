create proc [EducationPublisher].[VocabChoiceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@VocabQuestionId uniqueidentifier
    ,@Word nvarchar(50)
    ,@IsCorrect bit
    ,@AudioFileId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Education].[VocabChoice]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Word = @Word
    ,IsCorrect = @IsCorrect
    ,AudioFileId = @AudioFileId
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction